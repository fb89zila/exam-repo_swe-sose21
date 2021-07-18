using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Globalization;

[assembly: InternalsVisibleTo("MarkdownToLatex.Test")]

namespace MarkdownToLatex
{
    /// <summary>Main Class - coordinates which Markdown line is converted to which LaTeX line.</summary>
    public static class MdToTex
    {
        //field
        /// <summary>Contains the currently used <see cref="Calculator"/> class.</summary>
        private static ICalculator calc;

        /// <summary>Contains the file path to the Markdown document is parsed.</summary>
        internal static string mdFilePath;

        //methods

        /// <summary>Takes a <paramref name="match"/> of a math element, decides
        /// which math element to process and what results to generate and
        /// writes them into the LaTeX document.</summary>
        internal static void convertMathElement(Match match)
        {
            switch(match.Groups[1].Value){
                case "svfunc":
                    Match svfunc = MathParser.MatchSVFunction(match.Groups[2].Value);
                    double param;
                    bool hasParam = double.TryParse(svfunc.Groups[1].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out param);
                    calc = new FuncCalculator(svfunc.Groups[3].Value, svfunc.Groups[2].Value, hasParam ? param : null);
                    LatexRenderer.WriteMathElement(calc.ConvertElement());

                    MatchCollection mc = MathParser.MatchParameters(match.Groups[3].Value);
                    foreach(Match m in mc){
                        switch(m.Groups[1].Value){
                            case "result":
                                LatexRenderer.WriteMathElement((calc as FuncCalculator).Calculate());
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>Checks type of given Markdown <paramref name="text"/> and converts it to LaTeX.</summary>
        /// <returns>LaTeX lines of given Markdown <paramref name="text"/>.</returns>
        internal static void convertText(string text)
        {
            string bcText = LatexRenderer.WriteCursive(LatexRenderer.WriteBold(text)).TrimStart();

            if (bcText.StartsWith('#')) {
                LatexRenderer.WriteHeadline(MarkdownParser.MatchHeadline(bcText));
            } else if (bcText.StartsWith('-') || bcText.StartsWith('+')) {
                LatexRenderer.WriteList(MarkdownParser.MatchList(text));
            } else if (bcText.StartsWith('>')) {
                LatexRenderer.WriteQuote(MarkdownParser.MatchQuote(text));
            } else if (bcText.EndsWith("  ")) {
                LatexRenderer.StartNewLine(text);
            } else if (String.IsNullOrWhiteSpace(bcText)) {
                LatexRenderer.StartNewParagraph();
            } else {
                LatexRenderer.WriteText(text);
            }
        }

        /// <summary>Desides if given <paramref name="line"/> is Markdown text or a custom math element.</summary>
        /// <returns>LaTeX line to be saved in <see cref="LatexRenderer.LatexLines"/>.</returns>
        internal static void convert(string line)
        {
            Match lineMatch = MarkdownParser.MatchMathElement(line);

            if (lineMatch != Match.Empty) {
                convertMathElement(lineMatch);
            } else {
                convertText(line);
            }
        }

        /// <summary>Parses the <paramref name="inputPath"/> given as argument.</summary>
        /// <returns>Usable path to a Markdown file.</returns>
        internal static string parsePath(string inputPath)
        {
            if (File.Exists(inputPath)) {
                if (Path.GetExtension(inputPath) == ".md") {
                    return inputPath;
                } else {
                    throw new FileNotFoundException("Only Markdown files can be converted.");
                }
            } else {
                throw new FileNotFoundException();
            }
        }
        
        /// <summary>Entrypoint</summary>
        /// <param name="args">[1]: path to Markdownfile</param>
        /// <param name="args">[2]: (maybe later LaTeX output)</param>
        static void Main(string[] args)
        {
            try {
                mdFilePath = parsePath(args[1]);
            } catch (FileNotFoundException e) {
                Console.WriteLine("There was a problem with given path:\n{0}", e.Message);
            } catch (Exception e) {
                Console.WriteLine("Error:\n{0}", e.Message);
            }

            MarkdownParser.ReadMdDocument(mdFilePath);

            try {
                foreach (string line in MarkdownParser.MdLines) {
                    convert(line);
                }
            } catch (Exception e) {
                Console.WriteLine("Error:\n{0}", e.Message);
            }
            
        }
    }
}
