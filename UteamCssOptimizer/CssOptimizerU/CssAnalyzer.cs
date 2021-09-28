using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using CssOptimizerU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CssOptimizerU
{
    public class CssAnalyzer
    {   
        public static async Task<CssUsingDataModel> AnalyzePage(string pageUrl)
        {
            var config = Configuration.Default.WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true }).WithCss();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(pageUrl);
            var cssFileName = "general.css";
            var sheets = GetAllCssFiles(document);

            Console.WriteLine("All css file on the page:\n");
            foreach (var styleSheet in sheets)
            {
                Console.WriteLine(styleSheet.Href);
            }

            var sheet = sheets.FirstOrDefault(sh => sh.Href.Contains(cssFileName));

            Console.WriteLine();
            Console.WriteLine("+++++++++++++++++ Analyze general.css ++++++++++++++++++++++++++++++++++ :\n");

            var docStyleData = await AnalyzeDocStyles(sheet);
            docStyleData.FileName = cssFileName;

            CssUsingDataModel cssUsingData = new CssUsingDataModel();
            cssUsingData.PageUrl = pageUrl;
            cssUsingData.DocStyles.Add(docStyleData);
            cssUsingData = CollectUsageStatistic(document, cssUsingData);

            return cssUsingData;
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



        private static async Task<DocStyle> AnalyzeDocStyles(IStyleSheet sheet)
        {
            DocStyle docStyleData = new DocStyle();
            Console.WriteLine();
            Console.WriteLine();

            if (sheet == null)
            {
                throw new Exception("sheet is null");
            }

            CssParser cssParser = new CssParser();
            if (sheet.Source != null)
            {
                var styleCssSheet = await cssParser.ParseStyleSheetAsync(sheet.Source.Text);

                Console.WriteLine("Parse Css: \n");
                docStyleData = ProcessRules(styleCssSheet.Rules, docStyleData);
            }
            else
            {

                Console.WriteLine("sheet.Source is null");
            }

            return docStyleData;
        }


        private static DocStyle ProcessRules(ICssRuleList rules, DocStyle docStyleData, string conditionText = "")
        {

            foreach (var rule in rules)
            {

                if (rule is ICssGroupingRule)
                {                
                    if (rule is ICssMediaRule)
                    {
                        ICssGroupingRule mediaRule = rule as ICssGroupingRule;
                    }
                    else {
                        ICssGroupingRule groupRule = rule as ICssGroupingRule;
                        docStyleData = ProcessRules(groupRule.Rules, docStyleData, ((ICssMediaRule)rule).ConditionText);
                    }         
                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(rule.Type);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(rule.CssText);
             

                    if (rule is ICssStyleRule)
                    {
                        var selectorText = ((ICssStyleRule)rule).SelectorText;
                        if (!string.IsNullOrEmpty(selectorText))
                        {
                            foreach (var selector in selectorText.Split(','))
                            {
                                docStyleData.Selectors.Add(new DocStyleSelector { FullRuleText = selectorText, Content = rule.CssText, Name = selector, ConditionText = conditionText });
                  
                            }
                        }
                    }
                }
            }

   

            return docStyleData;
        }

        private static CssUsingDataModel CollectUsageStatistic(IDocument document, CssUsingDataModel cssUsingDataModel)
        {
           // TODO: add pseud classes :active :focus logic
            foreach (var docStyle in cssUsingDataModel.DocStyles)
            {
                DocStyle usageDocStyle = new DocStyle();
                foreach (var selector in docStyle.Selectors) {

                    var usingCount = document.QuerySelectorAll(selector.Name).Length;
                    if (usingCount > 0) {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        usageDocStyle.Selectors.Add(selector);
                        selector.IsUsed = true;
                       
                    }
                    else {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                   
                    Console.WriteLine($"count elements by query for selector: {selector.Name} -  {usingCount}");
                }

                if (usageDocStyle.Selectors.Any()) {
                    usageDocStyle.FileName = docStyle.FileName;
                    cssUsingDataModel.UsageStyles.Add(usageDocStyle);
                }
                
            }

            return cssUsingDataModel;
        }
    }
}
