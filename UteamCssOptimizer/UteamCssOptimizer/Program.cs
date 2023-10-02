using CssOptimizerU;
using CssOptimizerU.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace UteamCssOptimizer
{
	class Program
	{
		static void Main(string[] args)
		{
			CssAnalyzeOptions options = new CssAnalyzeOptions { CssProcessFileNames = Array.Empty<string>(), IsProcessAllFiles = true, PageUrl = "http://bara.uteam-dev.com/" };

			var serilogLogger = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.WriteTo.Console().MinimumLevel.Debug()
			.CreateLogger();

			var loggerFactory = (ILoggerFactory)new LoggerFactory();
			loggerFactory.AddSerilog(serilogLogger);
			var cssAnalyzerLogger = loggerFactory.CreateLogger<CssAnalyzer>();
			var cssOptimizerLogger = loggerFactory.CreateLogger<CssOptimizer>();
	
			// step 1: analyze Data
			var cssUsingData = new CssAnalyzer(cssAnalyzerLogger).AnalyzePage(options);

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
			ICssAnalyzerDbService dbServcie = new CssAnalyzerDbService(connectionString);

			foreach (var cssData in cssUsingData.Result)
			{
				dbServcie.SaveCssData(cssData);
			}

			// step 3: Generate optimixed css
			CssOptimizer optimizer = new CssOptimizer(dbServcie, cssOptimizerLogger);
			optimizer.GenerateOptimizeCssFiles(settings.DestinationPath, options.PageUrl, settings.IgnoreFileCondition);

			Log.Debug("I am done!!!\n");		
		}
	}
}
