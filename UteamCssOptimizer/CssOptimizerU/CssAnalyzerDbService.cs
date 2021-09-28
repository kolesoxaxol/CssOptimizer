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
                    context.Files.Add(new DM.File { CreatedDate = DateTime.Now, Name = docStyle.FileName, UpdateDate = DateTime.Now });
                }
                context.SaveChanges();
            }

        }

    }
}
