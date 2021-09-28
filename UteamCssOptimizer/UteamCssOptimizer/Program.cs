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

            IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .AddCommandLine(args)
              .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            CssAnalyzerDbService dbServcie = new CssAnalyzerDbService();
            dbServcie.SaveCssData(cssUsingData.Result);

            Console.ReadLine();
        }
    }
}
