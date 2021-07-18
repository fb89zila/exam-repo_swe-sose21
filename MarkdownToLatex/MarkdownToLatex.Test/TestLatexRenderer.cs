using System.IO;
using Xunit;
using System;

namespace MarkdownToLatex.Test
{
    public class TestLatexRenderer
    {

        Random rnd = new Random();

        [Theory]
        [InlineData("# A nice Chapter", @"\chapter*{A nice Chapter}")]
        [InlineData("## A nicer Section", @"\section*{A nicer Section}")]
        [InlineData("### An even nicer Subsection", @"\subsection*{An even nicer Subsection}")]
        [InlineData("#### vërÿ wëïrd ïnpüt", @"\subsection*{vërÿ wëïrd ïnpüt}")]
        public void TestWriteHeadline(string mdline, string expected){
            //arrange
            string line = mdline;

            //act
            LatexRenderer.WriteHeadline(MarkdownParser.MatchHeadline(line));

            //assert
            Assert.Contains(expected, LatexRenderer.LatexLines);
        }

        [Fact]
        public void TestWriteList(){
            //arrange
            LatexRenderer.LatexLines.Clear();
            string[] mdlines = {
                "- Hello, this is a test",
                "  - with a list",
                "  - which really is very nice",
                "    - or is it?",
                "- ok, that was it for now!"
            };

            string[] expected = {
                @"\begin{itemize}",
                @"\item{Hello, this is a test}",
                @"\begin{itemize}",
                @"\item{with a list}",
                @"\item{which really is very nice}",
                @"\begin{itemize}",
                @"\item{or is it?}",
                @"\end{itemize}",
                @"\end{itemize}",
                @"\item{ok, that was it for now!}",
                @"\end{itemize}"
            };

            //act
            foreach(string s in mdlines){
                LatexRenderer.WriteList(MarkdownParser.MatchList(s));
            }

            //assert
            Assert.Equal(expected, LatexRenderer.LatexLines.ToArray());
        }

        [Fact]
        public void TestWriteQuote(){
            //arrange
            LatexRenderer.LatexLines.Clear();
            string[] mdlines = {
                "> Yesterday, I heard someone saying",
                "> something interesting to somebody.",
                ">> I have a very deep quote for you:",
                ">>> Life is what happens when you're busy making other plans.",
                ">>> - John Lennon",
                ">> I hope, you liked it!",
                "> Pretty interesting, right?"
            };

            string[] expected = {
                @"\begin{quote}",
                @"\quoteline{Yesterday, I heard someone saying}",
                @"\quoteline{something interesting to somebody.}",
                @"\begin{quote}",
                @"\quoteline{I have a very deep quote for you:}",
                @"\begin{quote}",
                @"\quoteline{Life is what happens when you're busy making other plans.}",
                @"\quoteline{- John Lennon}",
                @"\end{quote}",
                @"\quoteline{I hope, you liked it!}",
                @"\end{quote}",
                @"\quoteline{Pretty interesting, right?}",
                @"\end{quote}"
            };

            //act
            foreach(string s in mdlines){
                LatexRenderer.WriteQuote(MarkdownParser.MatchQuote(s));
            }

            //assert
            Assert.Equal(expected, LatexRenderer.LatexLines.ToArray());
        }

        [Theory]
        [InlineData("Hello *World*!", @"Hello \textit{World}!")]
        [InlineData("***WARNING:*** Please do **always** wear gloves (_important_!)", @"\textbf{\textit{WARNING:}} Please do \textbf{always} wear gloves (\textit{important}!)")]
        [InlineData("All good things are **three**, *not* two!", @"All good things are \textbf{three}, \textit{not} two!")]
        [InlineData("***a***b*c***d*e*f**g***h*i**j**k*l***m*n**o***p**q*r**s**t*u***v**w*x*y**z***", @"\textbf{\textit{a}}b\textit{c}\textbf{d\textit{e}f}g\textbf{\textit{h}i}j\textbf{k\textit{l}}m\textit{n\textbf{o}}p*\textit{q}r\textbf{s}t\textit{u}\textbf{v}w\textit{x}y\textbf{z}*")]
        public void TestBoldCursive(string mdinput, string expected){
            //arrange
            string line = mdinput;

            //act
            string result = LatexRenderer.WriteCursive(LatexRenderer.WriteBold(line));

            //assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestWriteLatexDocument(){
            //arrange
            string expPath = @"test_files";
            string expFilename = "test.tex";
            string[] expected = {
                @"\documentclass{scrreprt}",
                @"\setlength{\parindent}{0pt}",
                @"\newcommand{\quoteline}",
                @"",
                @"\begin{document}",
                @"\chapter*{A nice Chapter}",
                @"\section*{A nicer Section}",
                @"\subsection*{vërÿ wëïrd ïnpüt}",
                @"\subsection*{An even nicer Subsection}",
                @"\end{document}"
            };

            //act
            LatexRenderer.WriteLatexDocument(expPath, expFilename);
            string[] result = File.ReadAllLines(Path.Combine(expPath, $"latex/{expFilename}"));

            //assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestWriteText(){
            //arrange
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
            string randomText = "";
            int randomTextLength = rnd.Next(1, 256);
            for (int i = 0; i<randomTextLength; i++){
                randomText+=chars.ToCharArray()[rnd.Next(0, chars.Length)];
            }

            //act
            LatexRenderer.WriteText(randomText);

            //assert
            Assert.Contains(randomText, LatexRenderer.LatexLines);
        }

        [Fact]
        public void TestNewLine(){
            //arrange
            LatexRenderer.LatexLines.Clear();
            string[] texlines = {"Hello, this is very important: ", @"a new line will be started after this text.\\", "And there it is!"};

            //act
            LatexRenderer.WriteText("Hello, this is very important: ");
            LatexRenderer.StartNewLine("a new line will be started after this text.");
            LatexRenderer.WriteText("And there it is!");

            //assert
            Assert.Equal(texlines, LatexRenderer.LatexLines.ToArray());
        }

        [Fact]
        public void TestNewParagraph(){
            //arrange
            LatexRenderer.LatexLines.Clear();
            string[] texlines = {"This is a really wonderful sentence!", @"\\\\", "And now a different topic!"};

            //act
            LatexRenderer.WriteText("This is a really wonderful sentence!");
            LatexRenderer.StartNewParagraph();
            LatexRenderer.WriteText("And now a different topic!");

            //assert
            Assert.Equal(texlines, LatexRenderer.LatexLines.ToArray());
        }
    }
}