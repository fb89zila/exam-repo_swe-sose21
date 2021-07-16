using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace MarkdownToLatex {

    /// <summary>A static class used for parsing elements based on different regular expressions.</summary>
    public static class MathParser {

        /// <summary>The Dictionary containing regular expressions to parse math elements.</summary>
        public static Dictionary<string, Regex> MathRx {get;}

        /// <summary>Checks if an <paramref name="element"/> is a single variable function.</summary>
        /// <returns>a bool indicating whether the <paramref name="element"/> is a single variable function or not and the fitting <paramref name="match"/>.</returns>
        public static bool MatchSVFunction(string element, out Match match) {
            Regex svregex;
            bool getregex = MathRx.TryGetValue("svfunction", out svregex);
            if(getregex){
                match = svregex.Match(element);
                return true;
            } else {
                Console.WriteLine("[ERROR] Error getting RegEx.");
                match = Match.Empty;
                return false;
            }
        }

        /// <summary>Initializes the Dictionary and adds regular expressions to it.</summary>
        static MathParser() {
            MathRx = new Dictionary<string, Regex>()
            {
                {"svfunction", new Regex(@"f\(([a-z]|[\d\.]+)\)=([\d\^\+\-\*\/a-z]*):([a-z])")}
            };
        }
    }
}