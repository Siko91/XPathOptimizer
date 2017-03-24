using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XPathOptimizer.Main
{
    public class PathOptimizer<T>
    {
        private Func<T, IEnumerable<T>> nextNodesGetter;
        private Func<T, double> scoreMeasurer;
        private Action onIterationFinished;
        private Func<bool> checkFinished;

        private Dictionary<T, double> uncheckedNodes;
        private List<T> acceptedNodes;

        public PathOptimizer(Func<T, IEnumerable<T>> nextNodesGetter, Func<T, double> scoreMeasurer, Action onIterationFinished, Func<bool> checkFinished)
        {
            this.nextNodesGetter = nextNodesGetter;
            this.scoreMeasurer = scoreMeasurer;
            this.onIterationFinished = onIterationFinished;
            this.checkFinished = checkFinished;
        }

        public IEnumerable<T> FindBest(T initialNode, int count)
        {
            acceptedNodes = new List<T> { initialNode };
            uncheckedNodes = new Dictionary<T, double>();

            while (true)
            {
                // look for new nodes
                var nextNodes = acceptedNodes.SelectMany(node => nextNodesGetter(node));
                uncheckedNodes = uncheckedNodes.Keys
                    .Union(nextNodes)
                    .ToDictionary(
                        node => node,
                        node => scoreMeasurer(node));

                // select nodes wit lowest score as current (don't remove it from unchecked)
                acceptedNodes = uncheckedNodes
                    .OrderByDescending(n => n.Value)
                    .Select(n => n.Key)
                    .Take(count)
                    .ToList();

                onIterationFinished();

                // if finished - exit
                if (checkFinished())
                    break;
            }

            return acceptedNodes;
        }
    }

    public class PathOptimizerFactory
    {
        public static PathOptimizer<string> ForXPaths(XElement xml, string xpath, XPathPartGenerator[] nodeReplacers, XPathScoreRule[] scoreRules, int desiredCount)
        {
            int i = 0;
            int len = XPathFinder.Split(xpath).Length;
            int timesFindAllShortenedPathsCalled = 0;

            return new PathOptimizer<string>(
                nextNodesGetter: (p) =>
                {
                    if (i < len)
                    {
                        var results = XPathFinder.FindXpathsWithReplacedNode(XPathFinder.Split(p), i, xml, nodeReplacers);
                        return results.Select(r => XPathFinder.Merge(r));
                    }
                    else
                    {
                        var results = XPathFinder.FindAllShortenedPaths(p, xml);
                        return results;
                    }
                },
                scoreMeasurer: (p) => XPathScorer.GetScore(p, scoreRules),
                onIterationFinished: () =>
                {
                    if (i < len)
                        i++;
                    else
                        timesFindAllShortenedPathsCalled++;
                },
                checkFinished: () => timesFindAllShortenedPathsCalled >= desiredCount);
        }
    }
}
