using System.IO;
using Xunit;
using System;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

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
            LatexRenderer.LatexLines.Clear();
            string line = mdline;

            //act
            LatexRenderer.WriteHeadline(MarkdownParser.MatchHeadline(line));
            string[] result = LatexRenderer.LatexLines.ToArray();

            //assert
            Assert.Contains(expected, result);
        }

        [Fact]
        public void TestWriteList(){
            //arrange
            LatexRenderer.LatexLines.Clear();
            LatexRenderer.InList = 0;
            LatexRenderer.InQuote = 0;
            string[] mdlines = {
                "- Hello, this is a test",
                "  - with a list",
                "  - which really is very nice",
                "    - or is it?",
                "    - this is still in layer 3",
                "    - does it work?",
                "  - Adding more to extend test converage",
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
                @"\item{this is still in layer 3}",
                @"\item{does it work?}",
                @"\end{itemize}",
                @"\item{Adding more to extend test converage}",
                @"\end{itemize}",
                @"\item{ok, that was it for now!}",
                @"\end{itemize}",
            };

            //act
            foreach(string s in mdlines){
                LatexRenderer.WriteList(MarkdownParser.MatchList(s));
            }
            string[] result = LatexRenderer.LatexLines.ToArray();

            //assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestWriteQuote(){
            //arrange
            LatexRenderer.LatexLines.Clear();
            string[] mdlines = {
                "> Yesterday, I heard someone saying",
                "> something interesting to somebody.",
                ">> I have a very deep quote for you!",
                ">> Here it is:",
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
                @"\quoteline{I have a very deep quote for you!}",
                @"\quoteline{Here it is:}",
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
            string[] result = LatexRenderer.LatexLines.ToArray();

            //assert
            Assert.Equal(expected, result);
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
            LatexRenderer.LatexLines.Clear();
            string[] input = {
                @"\chapter*{A nice Chapter}",
                @"\section*{A nicer Section}",
                @"\subsection*{vërÿ wëïrd ïnpüt}",
                @"\subsection*{An even nicer Subsection}"
            };
            string expPath1 = @"test_files/latex/test1.tex";
            string expPath2 = @"test_files/testAll.md";
            MdToTex.mdFilePath = @"test_files/test2.md";
            string expPath3 = @"test_files/custom_dir";
            string[] expected = {
                @"\documentclass{scrreprt}",
                @"\setlength{\parindent}{0em}",
                @"\setlength{\parskip}{1em}",
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
            LatexRenderer.LatexLines.AddRange(input);
            LatexRenderer.WriteLatexDocument(expPath1);
            LatexRenderer.WriteLatexDocument(expPath2);
            LatexRenderer.WriteLatexDocument(expPath3);
            System.Threading.Thread.Sleep(1000);
            string[] result1 = File.ReadAllLines(expPath1);
            string[] result2 = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(expPath2), "latex", Path.GetFileNameWithoutExtension(expPath2) + ".tex"));
            string[] result3 = File.ReadAllLines(Path.Combine(expPath3, Path.GetFileNameWithoutExtension(MdToTex.mdFilePath) + ".tex"));

            //assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
            Assert.Equal(expected, result3);
        }

        [Fact]
        public void TestWriteText(){
            //arrange
            LatexRenderer.LatexLines.Clear();
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
            string[] result = LatexRenderer.LatexLines.ToArray();

            //assert
            Assert.Equal(texlines, result);
        }

        [Fact]
        public void TestNewParagraph(){
            //arrange
            LatexRenderer.LatexLines.Clear();
            string[] texlines = {"This is a really wonderful sentence!", @"\par", "And now a different topic!"};

            //act
            LatexRenderer.WriteText("This is a really wonderful sentence!");
            LatexRenderer.StartNewParagraph();
            LatexRenderer.WriteText("And now a different topic!");
            string[] result = LatexRenderer.LatexLines.ToArray();

            //assert
            Assert.Equal(texlines, result);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        [InlineData(4, 3)]
        public void TestInListInQuote(byte value, byte expected){
            //arrange
            byte input = value;

            //act
            LatexRenderer.InList = input;
            LatexRenderer.InQuote = input;

            //assert
            Assert.Equal(expected, LatexRenderer.InList);
            Assert.Equal(expected, LatexRenderer.InQuote);
        }
    }
}