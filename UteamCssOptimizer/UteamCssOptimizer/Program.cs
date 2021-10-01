using CssOptimizerU;
using Microsoft.Extensions.Configuration;
using System;


namespace UteamCssOptimizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = "http://corona.uteam-dev.com/";
            var cssUsingData =  CssAnalyzer.AnalyzePage(address);


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

            CssAnalyzerDbService dbServcie = new CssAnalyzerDbService();
            dbServcie.SaveCssData(cssUsingData.Result);

            // OptimizeCss here x_optimize.css
            CssOptimizer optimizer = new CssOptimizer(dbServcie);
            optimizer.GenerateOptimizeCssFiles(settings.DestinationPath, address,settings.IgnoreFileCondition);
            Console.ReadLine();
        }
    }
}
