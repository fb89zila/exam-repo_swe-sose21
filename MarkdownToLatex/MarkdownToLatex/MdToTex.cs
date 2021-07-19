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
            string tempstr = text.TrimStart();
            string bcText = LatexRenderer.WriteCursive(LatexRenderer.WriteBold(text));

            if (tempstr.StartsWith('#')) {
                LatexRenderer.WriteHeadline(MarkdownParser.MatchHeadline(bcText));
            } else if (tempstr.StartsWith('-') || tempstr.StartsWith('+')) {
                LatexRenderer.WriteList(MarkdownParser.MatchList(bcText));
            } else if (tempstr.StartsWith('>')) {
                LatexRenderer.WriteQuote(MarkdownParser.MatchQuote(bcText));
            } else if (tempstr.EndsWith("  ")) {
                LatexRenderer.StartNewLine(bcText);
            } else if (String.IsNullOrWhiteSpace(bcText)) {
                LatexRenderer.StartNewParagraph();
            } else {
                LatexRenderer.WriteText(bcText);
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

        /// <summary>Parses the <paramref name="inputPath"/> given as argument one.</summary>
        /// <returns>Usable path to a Markdown file.</returns>
        /// <exception cref="FormatException">Thrown when path to a file has a file extention that is not '.md'</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file in given path does not exist.</exception>
        internal static string parseInputPath(string inputPath)
        {
            string path = Path.GetFullPath(inputPath);

            if (File.Exists(path)) {
                if (Path.GetExtension(path) == ".md") {
                    return path;
                } else {
                    throw new FormatException("Only Markdown files can be converted.");
                }
            } else {
                throw new FileNotFoundException();
            }
        }

        /// <summary>Parses the <paramref name="outputPath"/> given as argument two.</summary>
        /// <returns>Usable path to location where the LaTeX file should be created.</returns>
        /// <exception cref="FormatException">Thrown when path to a file has a file extention that is not '.tex'.</exception>
        internal static string parseOutputPath(string outputPath)
        {
            if (Path.HasExtension(outputPath)) {
                if (Path.GetExtension(outputPath) == ".tex") {
                    return outputPath;
                } else {
                    throw new FormatException("Only LaTeX documents with the file extention '.tex' can be created.");
                }
            } else {
                return outputPath;
            }  
        }
        
        /// <summary>Entrypoint</summary>
        /// <param name="args">[1]: path to Markdownfile</param>
        /// <param name="args">[2]: (maybe later LaTeX output)</param>
        static void Main(string[] args)
        {
            foreach (string a in args) {
                Console.WriteLine(a + "   " + args.Length);
            }

            try {
                mdFilePath = parseInputPath(args[0]);
            } catch (FileNotFoundException e) {
                Console.WriteLine("There was a problem with given file:\n{0}", e.Message);
            } catch (ArgumentOutOfRangeException) {
                Console.WriteLine("Please input the path to a Markdown file (.md) as the first argument.");
            } catch (Exception e) {
                Console.WriteLine("Error:\n{0}", e.Message);
            }

            try {
                MarkdownParser.ReadMdDocument(mdFilePath);
            } catch (PathTooLongException) {
                Console.WriteLine("The given file path was too long, could not read Markdown file.");
            }

            try {
                foreach (string line in MarkdownParser.MdLines) {
                    convert(line);
                }
            } catch (Exception e) {
                Console.WriteLine("Error:\n{0}", e.Message);
            }

            if (2 <= args.Length) {
                try {
                    LatexRenderer.WriteLatexDocument(parseOutputPath(args[1]));
                } catch (FormatException e) {
                    Console.WriteLine("There was a problem with given file path:\n{0}", e.Message);
                } catch (PathTooLongException) {
                    Console.WriteLine("The given file path was too long, could not write LaTeX file.");
                } catch (Exception e) {
                    Console.WriteLine("Error:\n{0}", e.Message);
                }
            } else {
                LatexRenderer.WriteLatexDocument(mdFilePath);
            }
        }
    }
}
