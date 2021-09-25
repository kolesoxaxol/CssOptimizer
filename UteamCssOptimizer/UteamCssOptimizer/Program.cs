using CssOptimizerU;
using Microsoft.Extensions.Configuration;
using System;

namespace UteamCssOptimizer
{
    class Program
    {
        static void Main(string[] args)
        {

            CssAnalyzer.Demo();

            IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .AddCommandLine(args)
              .Build();


            var connectionString = configuration.GetConnectionString("DefaultConnection");

            using (var context = new CssAnalyzerContext("Server=localhost\\SQLEXPRESS; Database=UteamCssAnalyzer; Trusted_Connection=True; MultipleActiveResultSets=true"))
            {
                context.SaveChanges();
            }

            Console.ReadLine();
        }
    }
}
