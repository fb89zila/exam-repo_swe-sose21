using System;
using System.IO;
using Xunit;

namespace MarkdownToLatex.Test
{
    
    public class TestMdToTex
    {
        private readonly char dirSep = Path.DirectorySeparatorChar;
        
        [Fact]
        public void TestParsePath(/*string path, string expPath*/)
        {
            //arrange
            string fileName = "path_test.md";
            string path = @"test_files"; 
            string expPath = @"test_files/path_test.md";

            //act
            string filePath = Path.Combine(path, fileName);
            File.Create(filePath);
            string result = MdToTex.parsePath(filePath);

            //assert
            Assert.Equal(expPath, result);
        }
        
        [Fact]
        public void TestConvertMathElement()
        {
            //arrange
            LatexRenderer.LatexLines.Clear();
            string mathElement = "!{svfunc} f(3.1415)=x^2+3*x+5:x !{result}";
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
    }
}
