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
        public void TestConvert()
        {
            //arrange
            //act
            //assert
        }
    }
}
