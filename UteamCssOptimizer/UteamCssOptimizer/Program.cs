using CssOptimizerU;
using CssOptimizerU.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace UteamCssOptimizer
{
    class Program
    {
        static void Main(string[] args)
        {
            CssAnalyzeOptions options = new CssAnalyzeOptions { cssProcessFileNames = Array.Empty<string>(), isProcessAllFiles = true, pageUrl = "http://bara.uteam-dev.com/" };


            // step 1: analyze Data
            var cssUsingData = CssAnalyzer.AnalyzePage(options);
        
            IConfiguration configuration = new ConfigurationBuilder()
              .SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .AddCommandLine(args)
              .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            ApplicationSettings settings = new ApplicationSettings();
            configuration.GetSection(ApplicationSettings.ApplicationSettingsSectionName).Bind(settings);


            // step 2: Save data to Db
            CssAnalyzerDbService dbServcie = new CssAnalyzerDbService(connectionString);

            foreach (var cssData in cssUsingData.Result)
            {
                dbServcie.SaveCssData(cssData);
            }

            // step 3: Generate optimixed css
            CssOptimizer optimizer = new CssOptimizer(dbServcie);
            optimizer.GenerateOptimizeCssFiles(settings.DestinationPath, options.pageUrl, settings.IgnoreFileCondition);
            Console.WriteLine("I am done!!!");
            Console.ReadLine();
        }
    }
}
