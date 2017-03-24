using HtmlAgilityPack;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XPathOptimizer.Main
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            string xmlPath = null;
            string xPath = null;
            int count = 10;

            string xpathsFile = null;
            string[] paths = null;

            string[] attributes = null;
            bool measureSpeed = false;

            bool help = false;

            OptionSet options = new OptionSet()
            {
                { "x|xml=", "The path to the XML file", v =>
                    xmlPath = v },

                { "p|xpath=", "The XPath to the desired element (if only one path needs to be optimized)", v =>
                    xPath = v },

                { "f|xpathsfile=", "The path to a file containing multiple XPaths to be optimized (1 per line)", v =>
                    xpathsFile = v },

                { "c|count=", "The count of the sudgestions to be given for each XPath (default is 10)", (int v) =>
                    count = v },

                { "a|attr=", "List of attributes that are considered important and stable in your xml format, seperated by commas. (default: 'id,name')", v =>
                    {
                        attributes = v.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < attributes.Length; i++)
                            attributes[i] = attributes[i].Trim();
                    }
                },

                { "s|speed", "If 'True', the program will attempt to prioratize faster paths, even if other paths are more readable", v =>
                    measureSpeed = v != null },

                { "h|help", "Shows the help", v =>
                    help = v != null },
            };

            try
            {
                var extra = options.Parse(args);

                if (extra.Any())
                    Console.WriteLine("Unknown parameters will be ignored: " + string.Join(", ", extra));
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `--help' for more information.");
                return;
            }

            if (xmlPath == null
                || (xPath == null && xpathsFile == null)
                || count < 1)
            {
                Console.WriteLine("Wrong usage of paramerers. See help.");
                Console.WriteLine("[{0}] is '{1}'", nameof(xmlPath), xmlPath);
                Console.WriteLine("[{0}] is '{1}'", nameof(xPath), xPath);
                Console.WriteLine("[{0}] is '{1}'", nameof(xpathsFile), xpathsFile);
                Console.WriteLine("[{0}] is '{1}'", nameof(count), count);
                Console.WriteLine();
                help = true;
            }

            if (help)
            {
                ShowHelp(options);
                return;
            }

            if (xpathsFile != null)
                paths = File.ReadAllLines(xpathsFile).Select(p => p.Trim()).ToArray();

            OptimizePaths(
                GetXElement(xmlPath),
                paths ?? new[] { xPath },
                count,
                attributes ?? new string[] { "id", "name" },
                measureSpeed);
        }

        private static XElement GetXElement(string xmlPath)
        {
            try
            {
                return new XElement("Root", XElement.Load(xmlPath));
            }
            catch (XmlException)
            {
                Console.WriteLine("Failed to parse XML. Attempting to treat it as HTML");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(File.ReadAllText(xmlPath));

                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    XmlTextWriter writer = new XmlTextWriter(sw);
                    doc.Save(writer);
                }

                return XElement.Parse(sb.ToString());
            }
        }

        private static void ShowHelp(OptionSet options)
        {
            options.WriteOptionDescriptions(Console.Out);
        }

        private static void OptimizePaths(XElement xml, IEnumerable<string> paths, int count, string[] attributes, bool measureSpeed)
        {
            IEnumerable<XPathPartGenerator> pathGenerators = new[] { XPathPartGenerator.TagOnly() }
                .Concat(attributes.Select(a => XPathPartGenerator.TagWithAttribute(a)))
                .ToArray();

            IEnumerable<XPathScoreRule> scoreRules = new[] {
                XPathScoreRule.PeneltizeStar(),
                XPathScoreRule.PeneltizeIndex(scoreWeight: 2),
                XPathScoreRule.PeneltizeStarWithIndex(),
                XPathScoreRule.TagUniqueness(xml),
                XPathScoreRule.PeneltizeLackOfAttributes(scoreWeight: 1),
                XPathScoreRule.AttributeUniqueness(xml, scoreWeight: 1)
            };

            if (measureSpeed)
                scoreRules = scoreRules.Concat(new[] { XPathScoreRule.MeasureTime(xml) });

            var tasks = paths
                .ToDictionary(p => p, p => Task.Run(() =>
                {
                    var path = XPathFinder.FindSimplestXPath(p, xml);
                    scoreRules = scoreRules.Concat(new[] { XPathScoreRule.PeneltizeLength(path.Split('/').Length, 0.1, 0.4, 
                        scoreWeight: 2) });
                    return DoOptiize(xml, path, count, pathGenerators.ToArray(), scoreRules.ToArray());
                }));


            foreach (var task in tasks)
            {
                Console.WriteLine("Results for : " + task.Key + Environment.NewLine);
                foreach (var line in task.Value.Result)
                    Console.WriteLine(line);
                Console.WriteLine();
            }
        }
        
        private static IEnumerable<string> DoOptiize(XElement xml, string path, int count,
            XPathPartGenerator[] xPathPartGenerators,
            XPathScoreRule[] scoreRules)
        {
            PathOptimizer<string> pathOptimizer = PathOptimizerFactory.ForXPaths(xml, path, xPathPartGenerators, scoreRules, count);
            var bestPaths = pathOptimizer.FindBest(path, count);

            foreach (var scoredPath in XPathScorer.GetScoredXPaths(bestPaths, bestPaths.Count(), scoreRules))
            {
                yield return string.Format("{0:0.000}  =>  {1}", scoredPath.Value, scoredPath.Key);
            }
        }
    }
}
