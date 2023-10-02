using CssOptimizerU.DM;
using CssOptimizerU.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssOptimizerU
{
    public class CssOptimizer
    {
        private readonly ICssAnalyzerDbService _dbAnalyzeService;
        private readonly ILogger<CssOptimizer> _logger;
        public CssOptimizer(ICssAnalyzerDbService dbAnalyzeService, ILogger<CssOptimizer> logger) {

            _dbAnalyzeService = dbAnalyzeService;
            _logger = logger;
		}

        public async void GenerateOptimizeCssFiles(string destinationPath, string pageUrl, string ignoreList)
        {
            var fileList = _dbAnalyzeService.GetCssFileNames().ToList();

            // TODO: rewrite it to regexp
            foreach (var ignoreCondtition in ignoreList.Split(";"))
            {
                fileList = fileList.FindAll(x=>!x.Name.Contains(ignoreCondtition));
            }

            try
            {
                foreach (var file in fileList)
                {

                    var optimizedCss = GetOptimizedCss(file.Id, pageUrl);
                    var fullpath = $"{destinationPath}/optimized_{file.Name}";

                    // optimize it
                    await System.IO.File.WriteAllTextAsync(fullpath, optimizedCss);        
                }

            }
            catch (Exception ex)
            { 
                _logger.LogError("Creating file error:" + ex.Message);
            }
        }
        public string GetOptimizedCss(int fileId, string pageUrl)
        {
            var cssText = string.Empty;

            List<Usage> usages = _dbAnalyzeService.GetCssUsage(pageUrl, fileId).ToList();

            var cssRules = usages.Select(usage => new OptimizedCssRule { Content = usage.Selector.Content, CssRule = usage.Selector.FullRuleText, ConditionText = usage.Selector.ConditionText });
            List<MediaCss> _mediaCssList = new List<MediaCss>();

            foreach (var usage in cssRules) {

                if (string.IsNullOrWhiteSpace(usage.ConditionText))
                {
                    cssText += $"\n{usage.Content} ";
                }
                else 
                {
                    _mediaCssList = ProcessConditions(usage, _mediaCssList);
                }      
            }

            foreach (var cssMedia in _mediaCssList)
            {
                cssText += "\n@media "+ cssMedia.MediaSelectorName + "\n{\n";
                cssText += cssMedia.Value + "}\n";
            }

            return cssText;
        }

        private List<MediaCss> ProcessConditions(OptimizedCssRule optimizedCssRule, List<MediaCss> _mediaCssList) 
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
