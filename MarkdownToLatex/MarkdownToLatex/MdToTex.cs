using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MarkdownToLatex.Test")]

namespace MarkdownToLatex
{
    /// <summary>Main Class - coordinates which Markdown line is converted to which LaTeX line.</summary>
    public static class MdToTex
    {
        //field
        /// <summary>Contains the currently used <see cref="Calculator"/> class.</summary>
        private static ICalculator calc;

        /// <summary>Contains the file path to the Markdown document which will be parsed.</summary>
        private static string mdFilePath;

        //methods
        internal static void convertMathElement(string element)
        {
            //
        }

        /// <summary>Checks type of given Markdown <param name="text"> and converts it to LaTeX.</summary>
        /// <returns>LaTeX lines of given Markdown <param name="text">.</returns>
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

        /// <summary>Desides if given <param name="line"> is Markdown text or a custom math element.</summary>
        /// <returns>LaTeX line to be saved in <see cref="LatexRenderer.LatexLines">.</returns>
        internal static void convert(string line)
        {
            Match lineMatch = MarkdownParser.MatchMathElement(line);

            if (lineMatch != Match.Empty) {
                convertMathElement(lineMatch.Groups[1].Value);
            }
            convertText(line);
        }

        /// <summary>Parses the <param name="inputPath"> given as argument.</summary>
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
        /// <param name="args">[1]: path to Markdownfile  
        /// [2]: (maybe later LaTeX output)</param>
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
