using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XPathOptimizer.Main
{
    public class XPathFinder
    {
        public static IEnumerable<string> FindAllShortenedPaths(string path, XElement xml)
        {
            var nodes = path.Split("/".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var alternativePaths = new List<string[]>() { nodes };

            for (int i = 0; i < nodes.Length - 1; i++)
            {
                foreach (var alternative in new List<string[]>(alternativePaths))
                {
                    if (!alternative[i].Contains("*") &&
                        alternative[i].Contains("[@"))
                        continue;

                    if (alternative.Count(node => string.IsNullOrWhiteSpace(node)) > 0.1 * alternative.Count())
                        continue;

                    var clonedNodes = new List<string>(alternative).ToArray();
                    clonedNodes[i] = "";

                    var newPath = Merge(clonedNodes);

                    if (newPath.EndsWith("/"))
                        continue;
                    if (HasSingleResult(newPath, xml))
                        alternativePaths.Add(clonedNodes);
                }
            }

            return alternativePaths
                .Select(n => Merge(n))
                .GroupBy(p => p)
                .Select(g => g.Key);
        }

        public static IEnumerable<string[]> FindXpathsWithReplacedNode(string[] xpath, int nodeIndex, XElement xml,
            params XPathPartGenerator[] nodeReplacers)
        {
            var element = xml.XPathSelectElement(Merge(xpath.Take(nodeIndex + 1).ToArray()));

            var replacedNodes = nodeReplacers
                .Select(r => r.Replace(element))
                .Where(newNode => newNode != null);

            var newPaths = new List<string[]>();
            foreach (var item in replacedNodes)
            {
                newPaths.Add((string[])xpath.Clone());
                newPaths.Last()[nodeIndex] = item;
            }

            return newPaths.Where(newPath => HasSingleResult(Merge(newPath), xml));
        }

        public static string Merge(string[] nodes)
        {
            var path = '/' + string.Join("/", nodes);
            while (path.IndexOf("///") > -1)
                path = path.Replace("///", "//");
            return path;
        }

        public static string[] Split(string originXPath)
        {
            return originXPath.Split("/".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static string FindSimplestXPath(string originXPath, XElement xml)
        {
            EnsureSingleResult(originXPath, xml);
            var element = xml.XPathSelectElement(originXPath);

            var path = new LinkedList<XElement>();
            while (element.Parent != null)
            {
                path.AddFirst(element);
                element = element.Parent;
            }

            string xPath = "";
            foreach (var el in path)
            {
                var index = el.ElementsBeforeSelf()
                    .Where(e => e.Name.LocalName == el.Name.LocalName)
                    .Count();
                xPath += "/" + el.Name.LocalName + "[" + (index + 1) + "]";
            }

            EnsureSingleResult(xPath, xml);
            return xPath;
        }

        public static void EnsureSingleResult(string xPath, XElement xml)
        {
            var elements = xml.XPathSelectElements(xPath);
            if (elements.Count() != 1)
                throw new ArgumentException(
                    "XPath generated " + elements.Count() + " results. " +
                    "Should have been a single result: " + xPath);
        }

        public static bool HasSingleResult(string xPath, XElement xml)
        {
            var elements = xml.XPathSelectElements(xPath);
            return elements.Count() == 1;
        }
    }
}

