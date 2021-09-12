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

        public static async Task FirstExample()
        {
            //Use the default configuration for AngleSharp
            var config = Configuration.Default;

            //Create a new context for evaluating webpages with the given config
            var context = BrowsingContext.New(config);

            //Parse the document from the content of a response to a virtual request
            var document = await context.OpenAsync(req => req.Content("<h1>Some example source</h1><p>This is a paragraph element"));

            //Do something with document like the following
            Console.WriteLine("Serializing the (original) document:");
            Console.WriteLine(document.DocumentElement.OuterHtml);

            var p = document.CreateElement("p");
            p.TextContent = "This is another paragraph.";

            Console.WriteLine("Inserting another element in the body ...");
            document.Body.AppendChild(p);

            Console.WriteLine("Serializing the document again:");
            Console.WriteLine(document.DocumentElement.OuterHtml);
        }
        public static async Task SecondExample()
        {


            // Setup the configuration to support document loading
            var config = Configuration.Default.WithDefaultLoader();
            // Load the names of all The Big Bang Theory episodes from Wikipedia
            var address = "http://corona.uteam-dev.com/";
            // Asynchronously get the document
            var document = await BrowsingContext.New(config).OpenAsync(address);
            // This CSS selector gets the desired content
            var cellSelector = "h1";
            // Perform the query to get all cells with the content
            var cells = document.QuerySelectorAll(cellSelector);
            // We are only interested in the text - select it with LINQ
            var titles = cells.Select(m => m.TextContent);

            Console.WriteLine("Overall {0} titles found...", titles.Count());
            Console.OutputEncoding = Encoding.GetEncoding("Windows-1255"); ;
            foreach (var title in titles)
                Console.WriteLine("* {0}", title.Trim(new[] { '"' }));
        }
        public static async Task Example()
        {

            // Setup the configuration to support document loading
            var config = Configuration.Default.WithDefaultLoader().WithCss();
            // Load the names of all The Big Bang Theory episodes from Wikipedia
            var address = "http://corona.uteam-dev.com/";

            var document = await BrowsingContext.New(config).OpenAsync(address);




            Console.WriteLine("All styles files link \n");
            foreach (var styleFile in document.QuerySelectorAll("link[type='text/css']"))
            {
                Console.WriteLine("OuterHtml: \n");
                Console.WriteLine(styleFile.OuterHtml);

                Console.WriteLine("Link: \n");
                var linkToFile = styleFile.GetAttribute("href");
                Console.WriteLine(linkToFile);



                //var config = Configuration.Default.WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true }).WithCss();
                //var context = BrowsingContext.New(config);
                //var address = "http://www.example.com"; // any reason for dropping the protocol?
                //var document = await context.OpenAsync(address);
                //var sheet = document.QuerySelector<IHtmlLinkElement>("link[rel=stylesheet]")?.Sheet;


                var cssFile = await BrowsingContext.New(config).OpenAsync($"{address}/{linkToFile}");
                CssParser cssParser = new CssParser();
                var sheet = await cssParser.ParseStyleSheetAsync(cssFile.Source.Text);

                Console.WriteLine(sheet.Rules.Length);
                int countCommonSelectors = 0;
                int countUsedSelectors = 0;

                foreach (var rule in sheet.Rules.Where(x => x.Type == CssRuleType.Style))
                {

                    //if (rule is ICssFontFaceRule)
                    //{
                    //    Console.WriteLine(((ICssFontFaceRule)rule).Family);
                    //    Console.WriteLine($"+++++++++  - {++count}");
                    //}

                    //if (rule is ICssCharsetRule)
                    //{
                    //    Console.WriteLine(((ICssCharsetRule)rule).CssText);
                    //    Console.WriteLine($"+++++++++  - {++count}");
                    //}

                    //if (rule is ICssPageRule)
                    //{
                    //    Console.WriteLine(((ICssPageRule)rule).SelectorText);
                    //    Console.WriteLine($"+++++++++  - {++count}");
                    //}

                    if (rule is ICssStyleRule)
                    {

                        var selector = ((ICssStyleRule)rule).SelectorText;
                        if (!string.IsNullOrEmpty(selector))
                        {
                            Console.WriteLine($"count elements by query for selector: {selector} -  {document.QuerySelectorAll(selector).Length}");

                            Console.WriteLine($"+++++++++  - {++countCommonSelectors}");
                            if (document.QuerySelectorAll(selector).Length > 0)
                            {
                                countUsedSelectors++;
                            }
                        }
                    }
                }

                Console.WriteLine($"used Selectors coount { countUsedSelectors} , used persentage on page is {countUsedSelectors / (countCommonSelectors / 100)}");
            }
        }

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
            AnalyzeDocStyles(document, sheet);

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
     
        

        private static async void AnalyzeDocStyles(IDocument document, IStyleSheet sheet) {

            Console.WriteLine();
            Console.WriteLine();

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

                        selectorsCss.Add(((ICssStyleRule)rule).SelectorText);
                    }
                }           
            }

            Console.WriteLine(selectorsCss.Count);
            Console.WriteLine(selectorCounter);
        }
    }
}
