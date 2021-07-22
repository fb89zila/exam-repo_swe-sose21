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
                    calc = new FuncCalculator(svfunc.Groups[1].Value, svfunc.Groups[2].Value);
                    LatexRenderer.WriteMathElement(calc.ConvertElement());

                    MatchCollection mc = MathParser.MatchParameters(match.Groups[3].Value);
                    foreach(Match m in mc){
                        switch(m.Groups[1].Value){
                            case "result":
                                double param; 
                                int precision;
                                bool hasParam = double.TryParse(m.Groups[2].Value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out param);
                                bool hasPrecision = int.TryParse(m.Groups[3].Value, out precision);
                                LatexRenderer.WriteMathElement((calc as FuncCalculator).Calculate(hasParam ? param : 0, hasPrecision ? precision : 2));
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
            string bcText = LatexRenderer.WriteCursive(LatexRenderer.WriteBold(LatexRenderer.WriteVerbatim(text)));

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
        /// <param name="args">[1]: path to Markdownfile  
        /// [2]: [optional] path for LaTeX output file</param>
        static void Main(string[] args)
        {
            Console.WriteLine("\nMarkdown to LaTeX converter\n---------------------------\n");

            //Trying to find the Markdown file.
            try {
                mdFilePath = parseInputPath(args[0]);
            } catch (FileNotFoundException e) {
                Console.WriteLine("Error trying to find Markdown file:\nThere was a problem with given file: {0}\n", e.Message);
            } catch (ArgumentOutOfRangeException) {
                Console.WriteLine("Error trying to find Markdown file:\nPlease input the path to a Markdown file (.md) as the first argument.\n");
            } catch (Exception e) {
                Console.WriteLine("Error trying to find Markdown file: {0}\n", e.Message);
            }

            //Trying to read the Markdown lines from the file.
            try {
                Console.WriteLine("Converting Markdown document...");
                MarkdownParser.ReadMdDocument(mdFilePath);
            } catch (PathTooLongException) {
                Console.WriteLine("Error while reading Markdown file:\nThe given file path was too long, could not read Markdown file.\n");
            } catch (Exception e) {
                Console.WriteLine("Error while reading Markdown file: {0}\n", e.Message);
            }

            //Trying to convert the Markdown lines.
            for (int i = 0; i < MarkdownParser.MdLines.Length; i++) {
                try {
                    convert(MarkdownParser.MdLines[i]);
                } catch (ConvertElementException e) {
                    Console.WriteLine("\n'{0}' line {1} - {2}", Path.GetFileName(mdFilePath), i+1, e.Message);
                } catch (Exception e) {
                    Console.WriteLine("\n'{0}' line {1} - Error while converting: {0}\n", Path.GetFileName(mdFilePath), i+1, e.Message);
                }
            }

            //Trying to save LaTeX file.
            //Inner try: Try to save file with the path given as command line argument two.
            //Outer try: Try to save file in the same directory as the Markdown file in a subdirectory "latex".
            try {
                if (2 <= args.Length) {
                    try {
                        string latexPath = parseOutputPath(args[1]);
                        LatexRenderer.WriteLatexDocument(latexPath);
                        Console.WriteLine("Saved '{0}' to: {1}", Path.GetFileName(latexPath), Path.GetDirectoryName(latexPath));
                    } catch (FormatException e) {
                        Console.WriteLine("Error while creating LaTeX file:\nThere was a problem with given file path: " +
                                          "{0}\nTrying to save file to the directory with Markdown file...\n", e.Message);
                        LatexRenderer.WriteLatexDocument(mdFilePath);
                    } catch (PathTooLongException) {
                        Console.WriteLine("Error while creating LaTeX file:\nThe given file path for the LaTeX document was too long.\n" +
                                          "Trying to save file to the directory with Markdown file...\n");
                        LatexRenderer.WriteLatexDocument(mdFilePath);
                    } catch (Exception e) {
                        Console.WriteLine("Error while creating LaTeX file: {0}\nTrying to save file to the directory with Markdown file...\n", e.Message);
                        LatexRenderer.WriteLatexDocument(mdFilePath);
                    }
                } else {
                    LatexRenderer.WriteLatexDocument(mdFilePath);
                    Console.WriteLine("Saved LaTeX file to: {0}", Path.GetFullPath(mdFilePath));
                }
            } catch (PathTooLongException) {
                Console.WriteLine("Error while creating LaTeX file: The resulting path was too long.\nFile could not be saved.\n");
            } catch (Exception e) {
                Console.WriteLine("Error while creating LaTeX file: {0}\nFile could not be saved.\n", e.Message);
            }

            Console.WriteLine("\n-----------------\nEnd of conversion");
        }
    }
}
