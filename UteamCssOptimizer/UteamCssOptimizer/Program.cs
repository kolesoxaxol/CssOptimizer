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

            CssAnalyzeOptions options = new CssAnalyzeOptions { cssProcessFileNames = Array.Empty<string>(), isProcessAllFiles = true, pageUrl = "https://rapnet-staging.azurewebsites.net/" };

            var cssUsingData = CssAnalyzer.AnalyzePage(options);


            // TODO: check why config is null
            IConfiguration configuration = new ConfigurationBuilder()
              .SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .AddCommandLine(args)
              .Build();


            var connectionString = configuration.GetConnectionString("DefaultConnection");
            ApplicationSettings settings = new ApplicationSettings();
            configuration.GetSection(ApplicationSettings.ApplicationSettingsSectionName).Bind(settings);

            CssAnalyzerDbService dbServcie = new CssAnalyzerDbService(connectionString);

            foreach (var cssData in cssUsingData.Result)
            {
                dbServcie.SaveCssData(cssData);
            }

            CssOptimizer optimizer = new CssOptimizer(dbServcie);
            optimizer.GenerateOptimizeCssFiles(settings.DestinationPath, options.pageUrl, settings.IgnoreFileCondition);
            Console.WriteLine("I am done!!!");
            Console.ReadLine();
        }
    }
}
