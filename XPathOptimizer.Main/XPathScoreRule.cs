using System;
using System.Collections.Generic;
using System.Xml.XPath;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace XPathOptimizer.Main
{
    public class XPathScoreRule
    {
        Func<string, double> scorePartGetter;
        int scoreWeight;
        private string ruleName;

        public XPathScoreRule(Func<string, double> scorePartGetter, int scoreWeight, [CallerMemberName]string ruleName = "")
        {
            this.ruleName = ruleName;
            this.scorePartGetter = scorePartGetter;
            this.scoreWeight = scoreWeight;
        }

        public double GetScorePart(string xpath, bool log = false)
        {
            var score = scorePartGetter(xpath);
            if (log)
                Console.WriteLine(ruleName + " Score : " + score * scoreWeight);
            return score * scoreWeight;
        }

        public static XPathScoreRule PeneltizeStar(int scoreWeight = 1)
        {
            return new XPathScoreRule((xpath) =>
            {
                var elements = xpath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var starCount = elements.Count(e => e == "*");
                return elements.Length / (starCount != 0 ? starCount : elements.Length);
            }, scoreWeight);
        }

        public static XPathScoreRule PeneltizeIndex(int scoreWeight = 1)
        {
            return new XPathScoreRule((xpath) =>
            {
                var elements = xpath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var indexCount = Regex.Matches(xpath, @"\[[0-9]+\]").Count;
                return ((double)elements.Count() - indexCount) / elements.Count();
            }, scoreWeight);
        }

        public static XPathScoreRule PeneltizeStarWithIndex(int scoreWeight = 1)
        {
            return new XPathScoreRule((xpath) =>
            {
                var elements = xpath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                return 1.0 / (1 + Regex.Matches(xpath, @"\*\[[0-9]+\]").Count);
            }, scoreWeight);
        }

        public static XPathScoreRule PeneltizeLength(int originLength, double minPreferedLengthPercantage = 0.2, double maxPreferedLengthPercantage = 0.5, int scoreWeight = 1)
        {
            return new XPathScoreRule((xpath) =>
            {
                var length = xpath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length;
                var goldenMiddle = (minPreferedLengthPercantage + maxPreferedLengthPercantage) / 2 * originLength;
                return Math.Abs(length - goldenMiddle) / length;
            }, scoreWeight);
        }

        static Dictionary<string, int> measuredTags = new Dictionary<string, int>();
        static int totalDescendants;
        public static XPathScoreRule TagUniqueness(XElement xml, int scoreWeight = 1)
        {
            totalDescendants = xml.Descendants().Count();

            return new XPathScoreRule((xpath) =>
            {
                var elements = xpath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                var scoreParts = elements.Select(node =>
                {
                    var tagName = node.Contains('[') ? node.Substring(0, node.IndexOf('[')) : node;

                    var tagRepetitions = 1;

                    if (tagName == "*")
                    {
                        return 1.0 / totalDescendants;
                    }
                    else if (measuredTags.ContainsKey(tagName))
                    {
                        tagRepetitions = measuredTags[tagName];
                    }
                    else
                    {
                        var nodesWithSameTagName = xml.Descendants(tagName);
                        tagRepetitions = nodesWithSameTagName.Count();
                        measuredTags.Add(tagName, tagRepetitions);
                    }

                    return (1.0 / tagRepetitions);
                });

                var scoreTotal = scoreParts.Average();
                return scoreTotal;
            }, scoreWeight);
        }

        const string attributeRegex = @"\/[^\/]+\[@(?<attr>[^=]+)='(?<val>[^']+)'\]";
        internal static XPathScoreRule PeneltizeLackOfAttributes(int scoreWeight = 1)
        {
            return new XPathScoreRule((xpath) =>
            {
                var length = xpath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length;

                var attributesCount = Regex.Matches(xpath, attributeRegex).Count;

                return ((double)attributesCount) / length;
            }, scoreWeight);
        }

        static Dictionary<string, int> measuredAttributes = new Dictionary<string, int>();
        public static XPathScoreRule AttributeUniqueness(XElement xml, int scoreWeight = 1)
        {
            return new XPathScoreRule((xpath) =>
            {
                var attributes = Regex.Matches(xpath, attributeRegex)
                    .OfType<Match>()
                    .Select(m => new KeyValuePair<string, string>(
                        m.Groups["attr"].Value,
                        m.Groups["val"].Value));

                if (!attributes.Any())
                {
                    return 0;
                }

                var counts = attributes.Select(attr =>
                {
                    var count = 1;
                    var dictKey = attr.Key + '=' + attr.Value;

                    if (measuredAttributes.ContainsKey(dictKey))
                    {
                        count = measuredAttributes[dictKey];
                    }
                    else
                    {
                        count = xml.Descendants()
                            .Where(el => el.Attribute(attr.Key) != null)
                            .Where(el => el.Attribute(attr.Key).Value == attr.Value)
                            .Count();
                        measuredAttributes.Add(dictKey, count);
                    }

                    return count;
                });

                return ((double)counts.Select((c, i) => 1 / c).Sum()) / counts.Count();

            }, scoreWeight);
        }

        public static XPathScoreRule MeasureTime(XElement xml, int repetitions = 30, int scoreWeight = 300)
        {
            return new XPathScoreRule((xpath) =>
               {
                   var watch = Stopwatch.StartNew();
                   for (int i = 0; i < repetitions; i++)
                       xml.XPathSelectElement(xpath);

                   watch.Stop();
                   return -watch.Elapsed.TotalSeconds;
               }, scoreWeight);
        }

    }
}

