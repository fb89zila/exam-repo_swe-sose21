using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MarkdownToLatex
{
    /// <summary>Class with methods to parse Markdown documents</summary>
    public static class MarkdownParser
    {
        //properties
        /// <summary>Contains Markdown lines. Read from <see cref="ReadMdDocument()"/>.</summary>
        public static string[] MdLines {get; private set;}

        /// <summary>
        /// Contains regular expressions defined in the constructor <see cref="MarkdownParser()"/>.  
        /// They are used to parse Markdown lines. For more details look <a href="https://github.com/fb89zila/exam-repo_swe-sose21/wiki/Developer#used-regular-expressions-">here</a>.
        /// </summary>
        public static Dictionary<string, Regex> TextRx {get;}

        //methods
        /// <summary>Reads lines of document from given file path. The resulting array is saved to <see cref="MdLines"/>.
        /// <seealso cref="MdToTex.parsePath()"></seealso></summary>
        public static void ReadMdDocument(string mdFilePath)
        {
            MdLines = File.ReadAllLines(mdFilePath);
        }

        /// <summary>Matches math element in Markdown with a custom "! " operator.</summary>
        /// <returns>Match with the math element in Group[1].</returns>
        public static Match MatchMathElement(string line)
        {
            return TextRx["math"].Match(line);
        }

        /// <summary>Matches headlines in Markdown.</summary>
        /// <returns>Match with the headline in Group[2] and type of headline as the length of Group[1].</returns>
        public static Match MatchHeadline(string text)
        {
            return TextRx["headline"].Match(text);
        }

        /// <summary>Matches list and sublist in Markdown.</summary>
        /// <returns>Match with the list item in Group[2] and the depth of the list as length/2 of Group[1]</returns>
        public static Match MatchList(string text)
        {
            return TextRx["list"].Match(text);
        }

        /// <summary>Matches quote in Markdown.</summary>
        /// <returns>Match with the quote in Group[2] and the depth of the quote as length of Group[1]</returns>
        public static Match MatchQuote(string text)
        {
            return TextRx["quote"].Match(text);
        }

        /// <summary>Matches cursive/italic text in Markdown line (when mixed, detect bold first and then cursive)</summary>
        /// <returns>MatchCollection with all the cursive/italic text in Group[1]</returns>
        public static MatchCollection MatchCursive(string text)
        {
            return TextRx["cursive"].Matches(text);
        }

        /// <summary>Matches bold text in a Markdown line (when mixed, detect bold first and then cursive)</summary>
        /// <returns>MatchCollection with all the bold text in Group[1]</returns>
        public static MatchCollection MatchBold(string text)
        {
            return TextRx["bold"].Matches(text);
        }

        //ctor
        /// <summary>Constructor initializes <see cref="TextRx"/></summary>
        static MarkdownParser()
        {
            TextRx = new Dictionary<string, Regex>()
            {
                {"math", new Regex(@"^! (.+)")},
                {"headline", new Regex(@"^(#+) (.+)")},
                {"list", new Regex(@"^((  )*)[-*+] (.+)")},
                {"quote", new Regex(@"^(>+) (.+)")},
                {"cursive", new Regex(@"(\*|_)([^\*_]+?|[^\*_]*?(\*\*|__)[^\*_]+?(\*\*|__)[^\*_]*?)(\1)")},
                {"bold", new Regex(@"(\*\*|__)([^\*_]+?|[^\*_]*?(\*|_)[^\*_]+?(\*|_)[^\*_]*?)(\1)")}
            };
        }
    }
}
