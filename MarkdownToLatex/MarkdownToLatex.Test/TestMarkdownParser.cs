using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Xunit;

namespace MarkdownToLatex.Test
{
    public class TestMarkdownParser
    {
        [Fact]
        public void TestReadMdDocument()
        {
            //arrange
            string path = @"test_files/test1.md";
            
            //act
            MarkdownParser.ReadMdDocument(path);

            //assert
            Assert.NotEmpty(MarkdownParser.MdLines);
        }

        [Theory]
        [InlineData("!{svfunc} f(x)=x^2+5*x-11/10:x !", "svfunc", "f(x)=x^2+5*x-11/10:x", "")]
        [InlineData("!{mvfunc} f(99.12,0)=x^4+3*x^3-111/100-1.2*y:x,y !{result}{root}", "mvfunc", "f(99.12,0)=x^4+3*x^3-111/100-1.2*y:x,y", "{result}{root}")]
        public void TestMatchMathElement(string teststr, string expType, string expText, string expParam)
        {
            //act
            Match result = MarkdownParser.MatchMathElement(teststr);

            //assert
            Assert.Equal(expType, result.Groups[1].Value);
            Assert.Equal(expText, result.Groups[2].Value);
            Assert.Equal(expParam, result.Groups[3].Value);
        }

        [Theory]
        [InlineData("# Headline", 1, "Headline")]
        [InlineData("## Smaller Headline", 2, "Smaller Headline")]
        [InlineData("### Even Smaller Headline", 3, "Even Smaller Headline")]
        public void TestMatchHeadline(string teststr, int expType, string expText)
        {
            //act
            Match result = MarkdownParser.MatchHeadline(teststr);

            //assert
            Assert.Equal(expType, result.Groups[1].Length);
            Assert.Equal(expText, result.Groups[2].Value);
        }

        [Theory]
        [InlineData(@"- some ***list*** item", 0, @"some ***list*** item")]
        [InlineData(@"  - some **sub**list item", 1, @"some **sub**list item")]
        [InlineData(@"    + some *subsub*list item", 2, @"some *subsub*list item")]
        public void TestMatchList(string teststr, int expLevel, string expText)
        {
            //act
            Match result = MarkdownParser.MatchList(teststr);
            //assert
            Assert.Equal(expLevel, result.Groups[1].Length/2);
            Assert.Equal(expText, result.Groups[2].Value);
        }

        [Theory]
        [InlineData("> s*om**e q**uot*e", 1, "s*om**e q**uot*e")]
        [InlineData(">> d***eeper q*uo**te", 2, "d***eeper q*uo**te")]
        [InlineData(">>> wo*w so d**ee***p", 3, "wo*w so d**ee***p")]
        public void TestMatchQuote(string teststr, int expDepth, string expText)
        {
            //act
            Match result = MarkdownParser.MatchQuote(teststr);
            //assert
            Assert.Equal(expDepth, result.Groups[1].Length);
            Assert.Equal(expText, result.Groups[2].Value);
        }

        [Theory]
        [InlineData("some *cursive* text", 1)]
        [InlineData("some*cursive*te_x_t", 2)]
        [InlineData("*s*o*m*e*c*ur*sive*te_x_t", 5)]
        [InlineData("*a*a*a*_a_*a*a*a*a_a_*a*a*a*", 8)]
        [InlineData("*a*a*a**a*a_a_*a*a*a*a_a__a_*a*a*a*", 10)]
        public void TestMatchCursive(string teststr, int matchCount)
        {
            //act
            MatchCollection result = MarkdownParser.MatchCursive(teststr);

            //assert
            Assert.Equal(matchCount, result.Count);
        }

        [Theory]
        [InlineData("some **bold** text", 1)]
        [InlineData("some**bold**te__x__t", 2)]
        [InlineData("**s**o**m**e**b**ol**d**te__x__t", 5)]
        [InlineData("**a**a**a**__a__**a**a**a**a__a__**a**a**a**", 8)]
        [InlineData("**a**a**a****a**a__a__**a**a**a**a__a____a__**a**a**a**", 10)]
        public void TestMatchBold(string teststr, int matchCount)
        {
            //act
            MatchCollection result = MarkdownParser.MatchBold(teststr);
            //assert
            Assert.Equal(matchCount, result.Count);
        }

        [Theory]
        [InlineData("***s***a*a***a*a*a**a***a*a**a**a*a***a*a**a***a**a*a**a**a*a***a**a*a*a**a***", 8 + 9)] // bold + cursive
        [InlineData("simple **bold**, *cursive* and ***bold+cursive***", 2 + 2)]
        public void TestMatchBoldAndCursive(string teststr, int matchCount)
        {
            //arrange
            MatchCollection boldMatch;
            MatchCollection cursiveMatch;
            string resultstr = teststr;
            int boldCounter;
            int cursiveCounter;

            //act
                //bold
            boldMatch = MarkdownParser.MatchBold(resultstr);
            boldCounter = boldMatch.Count;

            resultstr = MarkdownParser.TextRx["bold"].Replace(resultstr, @"\textbf{$2}");

                //cursive
            cursiveMatch = MarkdownParser.MatchCursive(resultstr);
            cursiveCounter = cursiveMatch.Count;

            resultstr = MarkdownParser.TextRx["cursive"].Replace(resultstr, @"\textit{$2}");

            Console.WriteLine(resultstr);
            //assert
            Assert.Equal(matchCount, boldCounter+cursiveCounter);
        }
    }
}
