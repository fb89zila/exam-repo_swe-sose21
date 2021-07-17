using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MarkdownToLatex
{
    static class MdToTex
    {
        //field
        /// <summary>Contains the currently used /// <see cref="Calculator"/> class</summary>
        private static ICalculator calc;

        /// <summary>Contains the file path to the Markdown document which will be parsed</summary>
        private static string mdFilePath;

        //methods
        private static string convertMathElement(string element)
        {
            return "";
        }

        /// <summary>Checks type of given Markdown <param name="text"> and converts it to LaTeX</summary>
        /// <returns>LaTeX lines of given Markdown <param name="text"></returns>
        private static string convertText(string text)
        {
            

            switch (String.IsNullOrEmpty(text)) {
                case false when MarkdownParser.MatchHeadline(text) != Match.Empty:
                    return ""; //temp
                case false when MarkdownParser.MatchList(text) != Match.Empty:
                    return ""; //temp
                case false when MarkdownParser.MatchQuote(text) != Match.Empty:
                    return ""; //temp
                case true: // `text` is null or empty --> new paragraph
                    return ""; //temp
                default:
                    return text;
            }
        }

        /// <summary>Desides if given line is Markdown text or a custom math element</summary>
        /// <returns>LaTeX line to be saved in <see cref="LatexRenderer.LatexLines"></returns>
        private static string convert(string line)
        {
            Match lineMatch = MarkdownParser.MatchMathElement(line);

            if (lineMatch != Match.Empty) {
                return convertMathElement(lineMatch.Groups[1].Value);
            }
            return convertText(line);
        }

        /// <summary>Parses the path given as argument</summary>
        /// <returns>Usable path to a Markdown file</returns>
        private static string parsePath(string inputPath)
        {
            char sep = Path.DirectorySeparatorChar;
            string current = Directory.GetCurrentDirectory();
            string path;

            if (Path.IsPathFullyQualified(inputPath)) {
                path = inputPath;
            } else {
                path = Path.Combine(current, inputPath);
            }

            if (File.Exists(path)) {
                if (Path.GetExtension(path) == ".md") {
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
        /// [2]: -</param>
        static void Main(string[] args)
        {
            try {
                MarkdownParser.ReadMdDocument(parsePath(args[1]));
            } catch (FileNotFoundException e) {
                Console.WriteLine("There was a problem with given path:\n{0}", e.Message);
            } catch (Exception e) {
                Console.WriteLine("Error:\n{0}", e.Message);
            }

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
