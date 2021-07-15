using System;
using Xunit;
using System.Text.RegularExpressions;

namespace MarkdownToLatex.Test
{
    public class TestCalculators
    {
        [Fact]
        public void TestEvaluateFunction()
        {
            Calculator calc = new FuncCalculator();
            string result = (calc as FuncCalculator).ConvertElement(13.37, "x", "2*x^3-7*x^2+5*x-3");

            Assert.Contains("3592.51", result);
        }

        private readonly Xunit.Abstractions.ITestOutputHelper hlp;

        public TestCalculators(Xunit.Abstractions.ITestOutputHelper helper){
            this.hlp = helper;
        }

        [Fact]
        public void TestMatchFunction(){
            Match mathMatch = MarkdownParser.MatchMathElement("! f(12.43)=x^2+5*x-11/10:x");

            Assert.NotEqual(mathMatch, Match.Empty);

            Match funcMatch = MathParser.MatchSVFunction(mathMatch.Groups[1].Value);

            Assert.NotEqual(funcMatch, Match.Empty);

            FuncCalculator fc = new FuncCalculator();

            double number;
            string result;

            if(double.TryParse(funcMatch.Groups[1].Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out number)){
                result = fc.ConvertElement(number, funcMatch.Groups[3].Value, funcMatch.Groups[2].Value);
            } else {
                result = fc.ConvertElement(funcMatch.Groups[3].Value, funcMatch.Groups[2].Value);
            }

            Assert.Contains("215.55", result);
        }
    }
}
