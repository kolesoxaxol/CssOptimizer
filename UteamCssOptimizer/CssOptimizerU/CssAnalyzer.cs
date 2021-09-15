using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssOptimizerU
{
    public class CssAnalyzer
    {
        public static IList<string> selectorsCss = new List<string>();

        public static int selectorCounter;
        public static async Task Demo()
        {
            var config = Configuration.Default.WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true }).WithCss();
            var context = BrowsingContext.New(config);
            var address = "http://corona.uteam-dev.com/";
            var document = await context.OpenAsync(address);

            var sheets = GetAllCssFiles(document);

            Console.WriteLine("All css file on the page:\n");
            foreach (var styleSheet in sheets)
            {
                Console.WriteLine(styleSheet.Href);
            }

            var sheet = sheets.FirstOrDefault(sh => sh.Href.Contains("general.css"));

            Console.WriteLine();
            Console.WriteLine("+++++++++++++++++ Analyze general.css ++++++++++++++++++++++++++++++++++ :\n");
          
            AnalyzeDocStyles(sheet);

            DisplayUsageStatistic(document);

            Console.ReadLine();
        }


        private static IEnumerable<IStyleSheet> GetAllCssFiles(IDocument document)
        {

            List<IStyleSheet> sheets = new List<IStyleSheet>();
            var links = document.QuerySelectorAll("link[rel=stylesheet]");

            foreach (var fileLink in links)
            {
                var sheet = ((IHtmlLinkElement)fileLink)?.Sheet;

                if (sheet != null)
                {
                    sheets.Add(sheet);
                };
            }

            return sheets;
        }
     
        

        private static async void AnalyzeDocStyles(IStyleSheet sheet) {

            Console.WriteLine();
            Console.WriteLine();

            if (sheet == null) {
                throw new Exception("sheet is null");
            }

            CssParser cssParser = new CssParser();
            if (sheet.Source != null)
            {
                var styleCssSheet = await cssParser.ParseStyleSheetAsync(sheet.Source.Text);

                Console.WriteLine("Parse Css: \n");
                ProcessRules(styleCssSheet.Rules);
            }
            else {
                Console.WriteLine("sheet.Source is null");
            }
        }


        private static void ProcessRules(ICssRuleList rules) {

            foreach (var rule in rules) {

                if (rule is ICssGroupingRule)
                {
                    ProcessRules(((ICssGroupingRule)rule).Rules);
                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(rule.Type);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(rule.CssText);
                    selectorCounter++;

                    if (rule is ICssStyleRule) {
                        var selectorText = ((ICssStyleRule)rule).SelectorText;
                        if (!string.IsNullOrEmpty(selectorText) ) {

                            selectorsCss.Add(selectorText);
                        }

                    }
                }           
            }

            Console.WriteLine(selectorsCss.Count);
            Console.WriteLine(selectorCounter);
        }

        private static void DisplayUsageStatistic(IDocument document) {

            foreach (var selectorText in selectorsCss)
            {
                foreach (var selector in selectorText.Split(',')) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"count elements by query for selector: {selector} -  {document.QuerySelectorAll(selector).Length}");
                }
            }
        }
    }
}
