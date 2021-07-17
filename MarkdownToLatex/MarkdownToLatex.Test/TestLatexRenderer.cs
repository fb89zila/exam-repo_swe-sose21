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
        public void TestWriteHeadline(string mdline, string texline){
            LatexRenderer.WriteHeadline(MarkdownParser.MatchHeadline(mdline));
            Assert.Contains(texline, LatexRenderer.LatexLines);
        }

        [Fact]
        public void TestWriteList(){
            LatexRenderer.LatexLines.Clear();
            string[] mdlines = {
                "- Hello, this is a test",
                "- with a list",
                "- which really is very nice",
                "- or is it?",
                "- ok, that was it for now!"
            };

            string[] texlines = {
                @"\begin{itemize}",
                @"\item{Hello, this is a test}",
                @"\item{with a list}",
                @"\item{which really is very nice}",
                @"\item{or is it?}",
                @"\item{ok, that was it for now!}",
                @"\end{itemize}"
            };

            foreach(string s in mdlines){
                LatexRenderer.WriteList(MarkdownParser.MatchList(s));
            }

            Assert.Equal(texlines, LatexRenderer.LatexLines.ToArray());
        }

        [Fact]
        public void TestWriteQuote(){
            LatexRenderer.LatexLines.Clear();
            string[] mdlines = {
                "> Life is what happens when you're busy making other plans.",
                "> - John Lennon"
            };

            string[] texlines = {
                @"\begin{quote}",
                "Life is what happens when you're busy making other plans.",
                "- John Lennon",
                @"\end{quote}"
            };

            foreach(string s in mdlines){
                LatexRenderer.WriteQuote(MarkdownParser.MatchQuote(s));
            }

            Assert.Equal(texlines, LatexRenderer.LatexLines.ToArray());
        }

        [Theory]
        [InlineData("Hello *World*!", @"Hello \textit{World}!")]
        [InlineData("***WARNING:*** Please do **always** wear gloves (_important_!)", @"\textbf{\textit{WARNING:}} Please do \textbf{always} wear gloves (\textit{important}!)")]
        [InlineData("All good things are **three**, *not* two!", @"All good things are \textbf{three}, \textit{not} two!")]
        public void TestBoldCursive(string mdinput, string texoutput){
            string result = LatexRenderer.WriteCursive(LatexRenderer.WriteBold(mdinput));
            Assert.Equal(texoutput, result);
        }

        [Fact]
        public void TestWriteLatexDocument(){
            string path = @"test_files/test.tex";
            LatexRenderer.WriteLatexDocument(path);
            Assert.True(File.Exists(path));
        }

        [Fact]
        public void TestWriteText(){
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
            string randomText = "";
            int randomTextLength = rnd.Next(1, 256);
            for (int i = 0; i<randomTextLength; i++){
                randomText+=chars.ToCharArray()[rnd.Next(0, chars.Length)];
            }

            LatexRenderer.WriteText(randomText);

            Assert.Contains(randomText, LatexRenderer.LatexLines);
        }

        [Fact]
        public void TestNewLine(){
            LatexRenderer.LatexLines.Clear();
            LatexRenderer.WriteText("Hello, this is very important: ");
            LatexRenderer.StartNewLine("a new line will be started after this text.");
            LatexRenderer.WriteText("And there it is!");

            string[] texlines = {"Hello, this is very important: ", @"a new line will be started after this text.\\", "And there it is!"};

            Assert.Equal(texlines, LatexRenderer.LatexLines.ToArray());
        }

        [Fact]
        public void TestNewParagraph(){
            LatexRenderer.LatexLines.Clear();
            LatexRenderer.WriteText("This is a really wonderful sentence!");
            LatexRenderer.StartNewParagraph();
            LatexRenderer.WriteText("And now a different topic!");

            string[] texlines = {"This is a really wonderful sentence!", @"\\\\", "And now a different topic!"};

            Assert.Equal(texlines, LatexRenderer.LatexLines.ToArray());
        }
    }
}