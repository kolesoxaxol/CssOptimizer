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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CssOptimizerU
{
    public class CssAnalyzer
    {
        public static async Task<List<CssUsingDataModel>> AnalyzePage(CssAnalyzeOptions options)
        {
			IConfiguration config = Configuration.Default.WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true}).WithCss();
			IBrowsingContext context = BrowsingContext.New(config);
			IDocument document = await context.OpenAsync(options.pageUrl);

            IEnumerable<IStyleSheet> sheets;
            List<CssUsingDataModel> cssUsingDataModels = new List<CssUsingDataModel>();

            if (options.isProcessAllFiles)
            {
                sheets = GetAllCssFiles(document);
            }
            else
            {
                // TODO optimize it with filter now we get all css files after filter it - need get just filterred list
                sheets = GetAllCssFiles(document).Where(p => options.cssProcessFileNames.All(fileName => p.Href.Contains(fileName)));
            }

            Console.WriteLine("All process css files on the page:\n");

            foreach (var styleSheet in sheets)
            {
                Console.WriteLine(styleSheet.Href);
            }

            if (!sheets.Any())
            {
                Console.WriteLine("sheet is empty");
                return cssUsingDataModels;
            }

            Regex reg = new Regex(@"(?:[^\/]*\/)*(.*?\.css)");

            foreach (var sheet in sheets)
            {
                CssUsingDataModel cssUsingData = new CssUsingDataModel();

                Console.WriteLine();
                Console.WriteLine("+++++++++++++++++ Analyze general.css ++++++++++++++++++++++++++++++++++ :\n");

                var docStyleData = await AnalyzeDocStyles(sheet);

                Match match = reg.Match(sheet.Href);


                docStyleData.FileName = match.Success ? match.Groups[1].Value : sheet.Href;

                cssUsingData.PageUrl = options.pageUrl;
                cssUsingData.DocStyles.Add(docStyleData);
                cssUsingData = CollectUsageStatistic(document, cssUsingData);

                cssUsingDataModels.Add(cssUsingData);
            }

            return cssUsingDataModels;
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

        // TODO: check why conditionText is empty in db
        private static DocStyle ProcessRules(ICssRuleList rules, DocStyle docStyleData, string conditionText = "")
        {

            foreach (var rule in rules)
            {

                if (rule is ICssGroupingRule)
                {
                    if (rule is ICssSupportsRule) 
                    {
                        continue;
                    }

                    if (rule is ICssMediaRule)
                    {
                        ICssGroupingRule mediaRule = rule as ICssGroupingRule;
                    }

                    if (rule is ICssKeyframesRule)
                    {

                        ICssKeyframesRule keyFrameRule = rule as ICssKeyframesRule;
                    }
                    else
                    {
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


					if (rule is ICssStyleRule cssStyleRule)
                    {
                        var selectorText = cssStyleRule.SelectorText;

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

            foreach (var docStyle in cssUsingDataModel.DocStyles)
            {
                DocStyle usageDocStyle = new DocStyle();
                foreach (var selector in docStyle.Selectors)
                {
                    string selectorName = selector.Name;
                    int usingCount;

                    //  check pseudo classes :active :focus logic
                    if (selectorName.Contains(":"))
                    {
                        // possible tag a[href^=\"javascript:\"]:after
                        var pseudoSelector = selector.Name.Split(":").LastOrDefault();
                        selectorName = selector.Name.Replace($":{pseudoSelector}", string.Empty);

                    }

                    // TODO there are extra cases svg:not(:root) audio:not([controls]) a[href^=\"javascript:\"]:after we count it used now
                    try
                    {

                        usingCount = document.QuerySelectorAll(selectorName).Length;

                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Wrong tag after parsing: {selector.Name} {selectorName}");
                        usingCount = 1;
                    }

                    if (usingCount > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        usageDocStyle.Selectors.ToList().Add(selector);
                        selector.IsUsed = true;

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    Console.WriteLine($"count elements by query for selector: {selector.Name} -  {usingCount}");

                }

                if (usageDocStyle.Selectors.Any())
                {
                    usageDocStyle.FileName = docStyle.FileName;
                    cssUsingDataModel.UsageStyles.ToList().Add(usageDocStyle);
                }

            }

            return cssUsingDataModel;
        }
    }
}
