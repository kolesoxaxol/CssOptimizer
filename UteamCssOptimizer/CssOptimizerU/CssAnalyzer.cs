using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssOptimizerU
{
    public class CssAnalyzer
    {
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

        public static async Task Demo()
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
                int count = 0;
               
                foreach ( var rule in sheet.Rules.Where(x=>x.Type == CssRuleType.Style)) {

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

                    if (rule is ICssPageRule)
                    {
                        Console.WriteLine(((ICssPageRule)rule).SelectorText);
                        Console.WriteLine($"+++++++++  - {++count}");
                    }

                    if (rule is ICssStyleRule) {

                        Console.WriteLine(((ICssStyleRule)rule).SelectorText);
                        Console.WriteLine($"+++++++++  - {++count}");
                    }


                }
            }
        }
    }
}
