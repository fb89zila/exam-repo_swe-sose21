using System.IO;
using Xunit;

namespace MarkdownToLatex.Test
{
    public class TestLatexRenderer
    {

        [Theory]
        [InlineData("# Der Tragödie erster Teil", @"\chapter*{Der Tragödie erster Teil}")]
        [InlineData("## Vor dem Tor - Osterspaziergang", @"\section*{Vor dem Tor - Osterspaziergang}")]
        [InlineData("### Alfred Ill", @"\subsection*{Alfred Ill}")]
        [InlineData("#### k0m!5(hЗr !npu7", @"\subsection*{k0m!5(hЗr !npu7}")]
        public void TestWriteHeadline(string mdline, string texline){
            LatexRenderer.WriteHeadline(MarkdownParser.MatchHeadline(mdline));
            Assert.Contains(texline, LatexRenderer.LatexLines);
        }

        [Fact]
        public void TestWriteList(){
            LatexRenderer.LatexLines.Clear();
            string[] mdlines = {
                "- Hallo, dies ist ein Test",
                "- mit einer Liste",
                "- die wirklich sehr, sehr toll ist",
                "- nicht wahr?",
                "- Ok, das war's auch schon!"
            };

            string[] texlines = {
                @"\begin{itemize}",
                @"\item{Hallo, dies ist ein Test}",
                @"\item{mit einer Liste}",
                @"\item{die wirklich sehr, sehr toll ist}",
                @"\item{nicht wahr?}",
                @"\item{Ok, das war's auch schon!}",
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
                "> Probleme kann man niemals mit derselben Denkweise lösen, durch die sie entstanden sind.",
                "> - Albert Einstein"
            };

            string[] texlines = {
                @"\begin{quote}",
                "Probleme kann man niemals mit derselben Denkweise lösen, durch die sie entstanden sind.",
                "- Albert Einstein",
                @"\end{quote}"
            };

            foreach(string s in mdlines){
                LatexRenderer.WriteQuote(MarkdownParser.MatchQuote(s));
            }

            Assert.Equal(texlines, LatexRenderer.LatexLines.ToArray());
        }

        [Theory]
        [InlineData("Das hast *du* gesagt, __nicht__ ich!", @"Das hast \textit{du} gesagt, \textbf{nicht} ich!")]
        [InlineData("***WARNUNG:*** Bitte **immer** Handschuhe tragen (_keine_ Stoffhandschuhe!)", @"\textbf{\textit{WARNUNG:}} Bitte \textbf{immer} Handschuhe tragen (\textit{keine} Stoffhandschuhe!)")]
        [InlineData("Alle guten Dinge sind **drei**, *nicht* zwei!", @"Alle guten Dinge sind \textbf{drei}, \textit{nicht} zwei!")]
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
    }
}