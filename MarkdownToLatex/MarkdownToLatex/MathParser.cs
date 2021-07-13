using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace MarkdownToLatex {

    /// <summary>
    /// A static class used for parsing elements based on different regular expressions
    /// </summary>
    public static class MathParser {

        /// <summary>
        /// The Dictionary containing regular expressions to parse math elements.
        /// </summary>
        public static Dictionary<string, Regex> mathrx {get;}

        /// <summary>
        /// Checks if an <paramref name="element"/> is a single variable function.
        /// </summary>
        /// <returns>
        /// a bool indicating whether the <paramref name="element"/> is a single variable function or not.
        /// </returns>
        /// <param name="element">The element to check.</param>
        public static bool IsSVFunction(string element) {
            Regex svregex;
            bool getregex = mathrx.TryGetValue("svfunction", out svregex);
            if(getregex){
                return svregex.IsMatch(element);
            } else {
                Console.WriteLine("[ERROR] Error getting RegEx.");
                return false;
            }
        }

        /// <summary>
        /// Initializes the Dictionary and adds regular expressions to it.
        /// </summary>
        static MathParser() {
            mathrx = new Dictionary<string, Regex>();
            mathrx.Add("svfunction", new Regex(@"f\(([a-z]|[\d\.]+)\)=([\d\^\+\-\*\\a-z]*):([a-z])"));
        }
    }
}