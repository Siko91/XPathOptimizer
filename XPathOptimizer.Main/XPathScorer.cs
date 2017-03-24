using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace XPathOptimizer.Main
{
	public class XPathScorer
	{
		public static Dictionary<string, double> GetScoredXPaths(IEnumerable<string> xPaths, int count, params XPathScoreRule[] rules)
		{
			return xPaths
				.ToDictionary (xp => xp, xp => GetScore (xp, rules))
				.OrderByDescending (scored => scored.Value)
				.Take (count)
				.ToDictionary (kv => kv.Key, kv => kv.Value);
		}
        
		public static double GetScore(string xPath, params XPathScoreRule[] rules)
		{
            if (!rules.Any())
            {
                return 0;
            }

			return rules.Select(r => r.GetScorePart(xPath)).Sum();
		}
	}
}

