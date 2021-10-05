using CssOptimizerU.DM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CssOptimizerU
{
    public class CssOptimizer
    {
        private readonly CssAnalyzerDbService _dbAnalyzeService;
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

                    //  Logger.Info(message);
                    //  Archive(JobStatus.Success, jobSettings.ArchiveFolder, path, fileId, true);
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


            // TODO: solve dublicate content when cut jsut one of the selector
            foreach (var usage in usages) {
                cssText += $"\n{usage.Selector.Content}";
            }

            return cssText;
        }
    }
}
