using CssOptimizerU.DM;
using CssOptimizerU.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssOptimizerU
{
    public class CssOptimizer
    {
        private readonly CssAnalyzerDbService _dbAnalyzeService;

        private List<MediaCss> _mediaCssList = new List<MediaCss>();
        public CssOptimizer(CssAnalyzerDbService dbAnalyzeService) {

            _dbAnalyzeService = dbAnalyzeService;
        }

        public async void GenerateOptimizeCssFiles(string destinationPath, string pageUrl, string ignoreList)
        {

            var fileList = _dbAnalyzeService.GetCssFileNames(pageUrl);

            // TODO: rewrite it to regexp
            foreach (var ignoreCondtition in ignoreList.Split(";"))
            {
                fileList = fileList.FindAll(x=>!x.Contains(ignoreCondtition));
            }

            try
            {
                foreach (var file in fileList)
                {

                    var optimizedCss = GetOptimizedCss(file, pageUrl);
                    var fullpath = $"{destinationPath}/optimized_{file}";

                    await System.IO.File.WriteAllTextAsync(fullpath, optimizedCss);
         
                }

            }
            catch (Exception ex)
            {   //TODO: add logs
                Console.WriteLine("Creating file error:" + ex.Message);
            }
        }
        public string GetOptimizedCss(string fileName, string pageUrl)
        {
            var cssText = string.Empty;

            List<Usage> usages = _dbAnalyzeService.GetCssUsage(pageUrl, fileName);

            var cssRules = usages.Select(usage => new OptimizedCssRule { Content = usage.Selector.Content, CssRule = usage.Selector.FullRuleText, ConditionText = usage.Selector.ConditionText}).Distinct();

            foreach (var usage in cssRules) {

                if (string.IsNullOrWhiteSpace(usage.ConditionText))
                {
                    cssText += $"\n{usage.Content}";
                }
                else 
                {
                    _mediaCssList = ProcessConditions(usage);
                }
          
            }

            foreach (var cssMedia in _mediaCssList)
            {
                cssText += "\n"+ cssMedia.MediaSelectorName + "\n{\n";
                cssText += cssMedia.Value + "}\n";
            }

            return cssText;
        }

        private List<MediaCss> ProcessConditions(OptimizedCssRule optimizedCssRule) 
        {
            var condition = _mediaCssList.FirstOrDefault(condition => condition.MediaSelectorName.Equals(optimizedCssRule.ConditionText));

            if (condition != null)
            {
                condition.Value += $"{optimizedCssRule.Content}\n";
                return _mediaCssList; 
            }

            _mediaCssList.Add(new MediaCss { MediaSelectorName = optimizedCssRule.ConditionText, Value = optimizedCssRule.Content });

            return _mediaCssList;
        }
    }
}
