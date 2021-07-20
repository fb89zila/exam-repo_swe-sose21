using System;
using System.IO;
using Xunit;

namespace MarkdownToLatex.Test
{
    
    public class TestMdToTex
    {        
        [Fact]
        public void TestParsePath(/*string path, string expPath*/)
        {
            //arrange
            char dirSep = Path.DirectorySeparatorChar;
            string mdPath = $@"test_files{dirSep}path_test.md";
            string texPath = $@"test_files{dirSep}latex{dirSep}path_test.tex";
            string expMdPath = Path.GetFullPath(mdPath);
            string expTexPath = $@"test_files{dirSep}latex{dirSep}path_test.tex";
            File.Create(mdPath);
            File.Create(texPath);

            //act
            string mdResult = MdToTex.parseInputPath(mdPath);
            string texResult = MdToTex.parseOutputPath(texPath);

            //assert
            Assert.Equal(expMdPath, mdResult);
            Assert.Equal(expTexPath, texPath);
        }
        
        [Fact]
        public void TestConvertMathElement()
        {
            //arrange
            LatexRenderer.LatexLines.Clear();
            string mathElement = "!{svfunc} f(x)=x^2+3*x+5:x !{result(3.1415)}";
            string[] expected = {
                @"\[f(x)=5 + 3x + {x}^{2}\]",
                @"\[f(3.1415)=24.29\]"
            };

            //act
            MdToTex.convertMathElement(MarkdownParser.MatchMathElement(mathElement));

            //assert
            foreach(string expLine in expected){
                Assert.Contains(expLine, LatexRenderer.LatexLines);
            }
        }

        [Fact]
        public void TestConvertText()
        {
            //arrange
            LatexRenderer.LatexLines.Clear();
            string[] mdLines = {
                "### head",
                ">> gucken",
                ">>> mal",
                "> test",
                "- wer",
                "  - wer",
                "- w",
                "sdf  ",
                "ass",
                ""
            };
            string[] expTex = {
                @"\subsection*{head}",
                @"\begin{quote}",
                @"\quoteline{gucken}",
                @"\begin{quote}",
                @"\quoteline{mal}",
                @"\end{quote}",
                @"\quoteline{test}",
                @"\end{quote}",
                @"\begin{itemize}",
                @"\item{wer}",
                @"\begin{itemize}",
                @"\item{wer}",
                @"\end{itemize}",
                @"\item{w}",
                @"\end{itemize}",
                @"sdf  \\",
                @"ass",
                @"\par"
            };

            //act
            foreach (string s in mdLines) {
                MdToTex.convertText(s);
            }
            string[] result = LatexRenderer.LatexLines.ToArray();

            //assert
            Assert.Equal(expTex, result);
        }

        [Fact]
        public void TestConvert()
        {
            //arranged 
            LatexRenderer.LatexLines.Clear();
            string[] mdLines = {
                "### head",
                ">> gucken",
                ">>> mal",
                "> test",
                "- wer",
                "  - wer",
                "- w",
                "sdf  ",
                "ass",
                "",
                "!{svfunc} f(x)=x^2+x-100:x !{result}  ",
                "!{svfunc} f(x)=x^4+3*x^3-111/100:x !{result(99.12)}"
            };
            string[] expTex = {
                @"\subsection*{head}",
                @"\begin{quote}",
                @"\quoteline{gucken}",
                @"\begin{quote}",
                @"\quoteline{mal}",
                @"\end{quote}",
                @"\quoteline{test}",
                @"\end{quote}",
                @"\begin{itemize}",
                @"\item{wer}",
                @"\begin{itemize}",
                @"\item{wer}",
                @"\end{itemize}",
                @"\item{w}",
                @"\end{itemize}",
                @"sdf  \\",
                @"ass",
                @"\par",
                @"\[f(x)=-100 + x + {x}^{2}\]",
                @"\[f(0)=-100\]",
                @"\[f(x)=-\frac{111}{100} + 3{x}^{3} + {x}^{4}\]",
                @"\[f(99.12)=99447685.82\]"
            };

            //act
            foreach (string s in mdLines) {
                MdToTex.convert(s);
            }
            string[] result = LatexRenderer.LatexLines.ToArray();

            //assert
            Assert.Equal(expTex, result);
		}
    }
}
