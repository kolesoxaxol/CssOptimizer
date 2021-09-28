using CssOptimizerU.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssOptimizerU
{
    public class CssAnalyzerDbService
    {
        private string _connectionString;
        private DbContextOptions<CssAnalyzerContext> _options;

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

                        if (selector.IsUsed)
                        {

                            DM.Usage usage = new DM.Usage
                            {
                                CreatedDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                PageUrl = cssUsingData.PageUrl,
                                Selector = cssSelector
                            };

                            context.Usages.Add(usage);
                        }
                    }

                }

                context.SaveChanges();
            }

        }

    }
}
