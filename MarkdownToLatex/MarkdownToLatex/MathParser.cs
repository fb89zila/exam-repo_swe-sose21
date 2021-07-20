using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace MarkdownToLatex {

    /// <summary>A static class used for parsing elements based on different regular expressions.</summary>
    public static class MathParser {

        /// <summary>The Dictionary containing regular expressions to parse math elements.</summary>
        public static Dictionary<string, Regex> MathRx {get;}

        public static MatchCollection MatchParameters(string parameters){
            return MathRx["params"].Matches(parameters);
        }

        /// <summary>Checks if an <paramref name="element"/> is a single variable function.</summary>
        /// <returns>a Match indicating whether the <paramref name="element"/> is a single variable function or not.</returns>
        public static Match MatchSVFunction(string element) {
            return MathRx["svfunction"].Match(element);
        }

        /// <summary>Initializes the Dictionary and adds regular expressions to it.</summary>
        static MathParser() {
            MathRx = new Dictionary<string, Regex>()
            {
                {"svfunction", new Regex(@"f\(([a-z]|[\d\.]+)\)=(.*):([a-z])")},
                {"params", new Regex(@"(?:{(?:([^{}\(\)]+)(?:\(((?:,?[\-\d\.]+)*)\))?)})")}
            };
        }
    }
}