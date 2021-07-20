using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.IO;

namespace MarkdownToLatex
{

    /// <summary>a static class containing methods used for rendering the LaTeX document.</summary>
    public static class LatexRenderer
    {
        /// <summary>a List containing the LaTeX document line by line.</summary>
        public static List<string> LatexLines;

        /// <seealso cref="InList"/>
        private static byte _inlist;

        /// <summary>Contains list level of last Markdown line ('0' if it was not in a list)</summary>
        private static byte oldInList;

        /// <summary>a byte indicating whether a list is currently rendered.</summary>
        public static byte InList {get => _inlist; private set {
            if(value <= 3){
                _inlist = value;
            } else {
                _inlist = 3; 
            }
        }}

        /// <seealso cref="InQuote"/>
        private static byte _inquote;

        /// <summary>Contains quote level of last Markdown line ('0' if it was not in a quote)</summary>
        private static byte oldInQuote;

        /// <summary>a byte indicating whether a quote is currently rendered.</summary>
        public static byte InQuote {get => _inquote; private set {
            if(value <= 3){
                _inquote = value;
            } else {
                _inquote = 3;
            }
        }}

        /// <summary>Resets the properties <see cref="InList"/> and <see cref="InQuote"/> to '0'.</summary>
        private static void ResetListOrQuote()
        {
            if (InList > 0) { InList = 0; }
            if (InQuote > 0) { InQuote = 0; }
        }

        /// <summary>Writes the LaTeX document with a <paramref name="filename"/> into a 'latex' folder at the specified <paramref name="path"/>.</summary>
        public static void WriteLatexDocument(string path){
            string latexPath;

            if(Path.GetExtension(path) == ".tex"){
                latexPath = path;
            } else if (Path.GetExtension(path) == ".md"){
                latexPath = Path.Combine(Path.GetDirectoryName(path), "latex", Path.GetFileNameWithoutExtension(path) + ".tex");
            } else {
                latexPath = Path.Combine(path, Path.GetFileNameWithoutExtension(MdToTex.mdFilePath) + ".tex");
            }
            Directory.CreateDirectory(Path.GetDirectoryName(latexPath));

            File.WriteAllLines(latexPath, new string[]{
                @"\documentclass{scrreprt}",
                @"\setlength{\parindent}{0em}",
                @"\setlength{\parskip}{1em}",
                @"\newcommand{\quoteline}" + "\n",
                @"\begin{document}"
            });
            File.AppendAllLines(latexPath, LatexLines);
            File.AppendAllText(latexPath, @"\end{document}");
        }

        /// <summary>Writes a MathElement in LaTeX.</summary>
        public static void WriteMathElement(string line){
            ResetListOrQuote();

            LatexLines.Add(@"\[" + line + @"\]"); //The calculation stuff is handled in MdToTex class
        }

        /// <summary>Writes a Headline in LaTeX using a Match <paramref name="m"/>.</summary>
        public static void WriteHeadline(Match m){
            ResetListOrQuote();

            int length = m.Groups[1].Value.Length; //First Group, number of #'s
            string caption = m.Groups[2].Value; //Second Group, the actual text

            switch(length){
                case 1:
                    LatexLines.Add(String.Format(@"\chapter*{{{0}}}", caption));
                    break;
                case 2:
                    LatexLines.Add(String.Format(@"\section*{{{0}}}", caption));
                    break;
                case 3:
                    LatexLines.Add(String.Format(@"\subsection*{{{0}}}", caption));
                    break;
                default:
                    LatexLines.Add(String.Format(@"\subsection*{{{0}}}", caption)); //Treat all further headlines as subsections in LaTeX
                    break;
            }
        }

        /// <summary>Writes a List in LaTeX using a Match <paramref name="m"/></summary>
        public static void WriteList(Match m){
            int depth = m.Groups[1].Value.Length/2+1;
            string content = m.Groups[2].Value;

            while(depth < InList){
                InList--;
            }

            if(InList == 0 && InList <= depth){ //if not in any list
                LatexLines.Add(@"\begin{itemize}");
                LatexLines.Add(String.Format(@"\item{{{0}}}", content));
                LatexLines.Add(@"\end{itemize}");
                InList = 1;
            } else if (InList == 1 && InList <= depth){
                if(depth == 1 && oldInList > InList){
                    appendNewItem(content, oldInList - InList);
                } else if(depth == 1 && oldInList == InList){
                    appendNewItem(content);
                } else {
                    newList(content);
                    InList++;
                }
            } else if (InList == 2 && InList <= depth){
                if(depth == 2 && oldInList > InList){
                    appendNewItem(content, oldInList - InList);
                } else if(depth == 2 && oldInList == InList){
                    appendNewItem(content);
                } else {
                    newList(content);
                    InList++;
                }
            } else if (InList == 3 && InList <= depth){
                appendNewItem(content);
            }
            oldInList = InList;
        }

        /// <summary>Appends a new item to an already existing list.</summary>
        /// <param name="content">The item to append</param>
        /// <param name="offset">Adds to the index where the new item should be inserted (for changing list level)</param>
        private static void appendNewItem(string content, int offset = 0){
            LatexLines.Insert(LatexLines.FindLastIndex(x => x.StartsWith(@"\item"))+offset+1, String.Format(@"\item{{{0}}}", content));
        }

        /// <summary>Starts a new list level with one item from <paramref name="content"/></summary>
        private static void newList(string content){
            List<string> tmp = new List<string>();
            tmp.Add(@"\begin{itemize}");
            tmp.Add(String.Format(@"\item{{{0}}}", content));
            tmp.Add(@"\end{itemize}");
            LatexLines.InsertRange(LatexLines.FindLastIndex(x => x.StartsWith(@"\item"))+1, tmp);
        }

        /// <summary>Writes a Quote in LaTeX using a Match <paramref name="m"/>.</summary>
        public static void WriteQuote(Match m){
            int depth = m.Groups[1].Value.Length;
            string content = m.Groups[2].Value;

            while(depth < InQuote){
                InQuote--;
            }

            if(InQuote == 0 && InQuote <= depth){ //if not in any quote
                LatexLines.Add(@"\begin{quote}");
                LatexLines.Add(String.Format(@"\quoteline{{{0}}}", content));
                LatexLines.Add(@"\end{quote}");
                InQuote = 1;
            } else if (InQuote == 1 && InQuote <= depth){
                if(depth == 1 && oldInQuote > InQuote){
                    appendNewQuoteLine(content, oldInQuote - InQuote);
                } else if(depth == 1 && oldInQuote == InQuote){
                    appendNewQuoteLine(content);
                } else {
                    newQuote(content);
                    InQuote++;
                }
            } else if (InQuote == 2 && InQuote <= depth){
                if(depth == 2 && oldInQuote > InQuote){
                    appendNewQuoteLine(content, oldInQuote - InQuote);
                } else if(depth == 2 && oldInQuote == InQuote){
                    appendNewQuoteLine(content);
                } else {
                    newQuote(content);
                    InQuote++;
                }
            } else if (InQuote == 3 && InQuote <= depth){
                appendNewQuoteLine(content);
            }
            oldInQuote = InQuote;
        }

        /// <summary>Appends a new line to an already existing quote.</summary>
        /// <param name="content">The line to append</param>
        /// <param name="offset">Adds to the index where the new line should be inserted (for changing quote level)</param>
        private static void appendNewQuoteLine(string content, int offset = 0){
            LatexLines.Insert(LatexLines.FindLastIndex(x => x.StartsWith(@"\quoteline"))+offset+1, String.Format(@"\quoteline{{{0}}}", content));
        }

        /// <summary>Starts a new quote level with one item from <paramref name="content"/></summary>
        private static void newQuote(string content){
            List<string> tmp = new List<string>();
            tmp.Add(@"\begin{quote}");
            tmp.Add(String.Format(@"\quoteline{{{0}}}", content));
            tmp.Add(@"\end{quote}");
            LatexLines.InsertRange(LatexLines.FindLastIndex(x => x.StartsWith(@"\quoteline"))+1, tmp);
        }

        /// <summary>Writes a <paramref name="line"/> with cursive text in LaTeX.</summary>
        /// <returns>the line, converted from Markdown into LaTeX.</returns>
        public static string WriteCursive(string line){
            return MarkdownParser.TextRx["cursive"].Replace(line, @"\textit{$2}");
        }

        /// <summary>Writes a <paramref name="line"/> with bold text in LaTeX.</summary>
        /// <returns>the line, converted from Markdown into LaTeX.</returns>
        public static string WriteBold(string line){
            return MarkdownParser.TextRx["bold"].Replace(line, @"\textbf{$2}");
        }

        /// <summary>Writes a normal text <paramref name="line"/> in LaTeX.</summary>
        public static void WriteText(string line){
            ResetListOrQuote();

            LatexLines.Add(line);
        }

        /// <summary>Starts a new line after the given <paramref name="line"/>.</summary>
        public static void StartNewLine(string line){
            ResetListOrQuote();

            LatexLines.Add(line + @"\\");
        }

        /// <summary>Starts a new paragraph.</summary>
        public static void StartNewParagraph(){
            ResetListOrQuote();
            
            LatexLines.Add(@"\par");
        }

        static LatexRenderer(){
            LatexLines = new List<string>();
        }
    }
}
