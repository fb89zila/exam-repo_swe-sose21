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
            Directory.CreateDirectory(Path.GetDirectoryName(texPath));
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
            string mathElement = "!{svfunc} f(x)=x^2+3*x+5 !{result(3.1415)}";
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
                "ass `\\ldots ist sehr schön`",
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
                @"sdf\\",
                @"ass \verb|\ldots ist sehr schön|",
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
                "!{svfunc} f(x)=x^2+x-100 !{result}  ",
                "!{svfunc} f(x)=x^4+3*x^3-111/100 !{result(99.12)}",
                "!{svfunc} f(x)=x^4+3*x^3-111/100 !{result(99.12)[5]}"
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
                @"sdf\\",
                @"ass",
                @"\par",
                @"\[f(x)=-100 + x + {x}^{2}\]",
                @"\[f(0)=-100\]",
                @"\[f(x)=-\frac{111}{100} + 3{x}^{3} + {x}^{4}\]",
                @"\[f(99.12)=99447685.82\]",
                @"\[f(x)=-\frac{111}{100} + 3{x}^{3} + {x}^{4}\]", 
                @"\[f(99.12)=99447685.81648\]"
            };

            //act
            foreach (string s in mdLines) {
                MdToTex.convert(s);
            }
            string[] result = LatexRenderer.LatexLines.ToArray();

            //assert
            Assert.Equal(expTex, result);
		}

        [Fact]
        public void TestParseInputPath(){
            //arrange
            string path1 = @"test_files/testAll.md";
            string path2 = @"test_files/latex/test1.tex";
            string path3 = @"test_files/nonexistantfile.md";

            //act
            Action parse1 = new Action(() => {string parsedPath1 = MdToTex.parseInputPath(path1);});
            Action parse2 = new Action(() => {string parsedPath2 = MdToTex.parseInputPath(path2);});
            Action parse3 = new Action(() => {string parsedPath3 = MdToTex.parseInputPath(path3);});
            parse1.Invoke();

            //assert
            Assert.Throws<FormatException>(parse2);
            Assert.Throws<FileNotFoundException>(parse3);
        }

        [Fact]
        public void TestParseOutputPath(){
            //arrange
            string path1 = @"test_files/latex";
            string path2 = @"test_files/latex/test1.tex";
            string path3 = @"test_files/nonexistantfile.md";
            Directory.CreateDirectory(Path.GetDirectoryName(path2));
            File.Create(path2).Close();

            //act
            Action parse1 = new Action(() => {string parsedPath1 = MdToTex.parseOutputPath(path1);});
            Action parse2 = new Action(() => {string parsedPath2 = MdToTex.parseOutputPath(path2);});
            Action parse3 = new Action(() => {string parsedPath3 = MdToTex.parseOutputPath(path3);});
            parse1.Invoke();
            parse2.Invoke();

            //assert
            Assert.Throws<FormatException>(parse3);
        }
    }
}
