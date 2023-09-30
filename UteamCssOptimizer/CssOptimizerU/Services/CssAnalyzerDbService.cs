using CssOptimizerU.DM;
using CssOptimizerU.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssOptimizerU
{
	public class CssAnalyzerDbService
    {
        private readonly string _connectionString;
        private readonly DbContextOptions<CssAnalyzerContext> _options;

        public CssAnalyzerDbService(string connectionString = "Server=localhost\\SQLEXPRESS; Database=UteamCssAnalyzer; Trusted_Connection=True; MultipleActiveResultSets=true")
        {
            _connectionString = connectionString;

            var optionsBuilder = new DbContextOptionsBuilder<CssAnalyzerContext>();

            _options = optionsBuilder
                    .UseSqlServer(_connectionString)
                    .Options;
        }

        public void SaveCssData(CssUsingDataModel cssUsingData)
        {

            using (var context = new CssAnalyzerContext(_options))
            {
                foreach (var docStyle in cssUsingData.DocStyles)
                {
                    var file = new DM.File { CreatedDate = DateTime.Now, Name = docStyle.FileName, UpdateDate = DateTime.Now };
                    context.Files.Add(file);
                    context.SaveChanges();

                    foreach (var selector in docStyle.Selectors)
                    {
                        var cssSelector = new DM.Selector
                        {
                            CreatedDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Content = selector.Content,
                            FullRuleText = selector.FullRuleText,
                            ConditionText = selector.ConditionText,
                            Name = selector.Name,
                            File = file
                        };

                        context.Selector.Add(cssSelector);
                        context.SaveChanges();

                        if (selector.IsUsed)
                        {

                            DM.Usage usage = new DM.Usage
                            {
                                CreatedDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                PageUrl = cssUsingData.PageUrl,
                                Selector = cssSelector,
                                File = file
                            };

                            context.Usages.Add(usage);
                            context.SaveChanges();
                        }
                    }

                }

            }

        }
        public List<File> GetCssFileNames()
        {
			using var context = new CssAnalyzerContext(_options);
			var fileNames = context.Files.ToList();

			return fileNames;
		}

        public List<Usage> GetCssUsage(string pageUrl, int fileId) {

            using (var context = new CssAnalyzerContext(_options))
            {
                var usages = context.Usages.Include(s=>s.Selector).Where(x => x.PageUrl.Equals(pageUrl) && x.File.Id == fileId).ToList();

                return usages;
            }
        }
    }
}
