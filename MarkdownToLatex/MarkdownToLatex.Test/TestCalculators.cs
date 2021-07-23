using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Xunit;

namespace MarkdownToLatex.Test
{
    public class TestCalculators
    {
        [Fact]
        public void TestCalculateSvFunc()
        {
            //arrange
            FuncCalculator calc = new FuncCalculator("x", "2*x^3-7*x^2+5*x-3");
            MatchCollection mc = MathParser.MatchParameters("{result(13.37)[0]}{result(13.37)}");
            List<string> result = new List<string>();
            string[] expected = new string[] {
                @"f(13.37)=3593",
                @"f(13.37)=3592.51"
            };

            //act
            foreach (Match m in mc) {
                result.Add(calc.Calculate(m));
            }

            //assert
            Assert.Equal(expected, result.ToArray());
        }

        [Fact]
        public void TestCalcRootSvFunc()
        {
            //arrange
            FuncCalculator calc = new FuncCalculator("x", "2*x^4-4*x^2+5*x-3");
            MatchCollection mc = MathParser.MatchParameters("{root(-2,0)[0]}{root(-2,0)}");
            List<string> result = new List<string>();
            string[] expected = new string[] {
                @"root([-2,0])=-2",
                @"root([-2,0])=-1.92"
            };

            //act
            foreach (Match m in mc) {
                result.Add(calc.CalcRoot(m));
            }

            //assert
            Assert.Equal(expected, result.ToArray());
        }

        [Fact]
        public void TestCalcDerivative()
        {
            //arrange
            FuncCalculator calc = new FuncCalculator("x", "2*x^4-4*x^2+5*x-3");
            MatchCollection mc = MathParser.MatchParameters("{d1f}{d1f(0)}{d1f(2e-3)[3]}{d2f}{d2f(0)}{d2f(2e-3)[3]}");
            List<string> result = new List<string>();
            string[] expected = new string[] {
                @"f'(x)=5 - 8x + 8{x}^{3}",
                @"f'(0)=5",
                @"f'(0.002)=4.984",
                @"f''(x)=-8 + 24{x}^{2}",
                @"f''(0)=-8",
                @"f''(0.002)=-8"
            };

            //act
            foreach (Match m in mc) {
                if (m.Groups[1].Value == "d1f") {
                    result.Add(calc.CalcDerivative(m, 1));
                } else {
                    result.Add(calc.CalcDerivative(m, 2));
                }
            }

            //assert
            Assert.Equal(expected, result.ToArray());
        }

        [Theory]
        [InlineData("x", "x^3-2*x^2+sin(x)", @"f(x)=-2{x}^{2} + {x}^{3} + \sin{x}")]
        [InlineData("t", "(sin(t))^2+(cos(t))^2", @"f(t)=\sin^{2}{t} + \cos^{2}{t}")]
        public void TestConvertFunction(string var, string function, string output)
        {
            //arrange
            FuncCalculator calc = new FuncCalculator(var, function);

            //assert
            Assert.Equal(output, calc.ConvertElement());
        }

        [Fact]
        public void TestExceptions(){
            //assert
            Assert.Throws<ConvertElementException>(() => {new FuncCalculator("e", "tanh(e**2)");});
            Assert.Throws<ConvertElementException>(() => {new FuncCalculator("y","x^2+2");});
        }
    }
}
