using Xunit;

namespace MarkdownToLatex.Test
{
    public class TestCalculators
    {
        [Fact]
        public void TestCalculateFunction()
        {
            FuncCalculator calc = new FuncCalculator("x", "2*x^3-7*x^2+5*x-3");
            string result = calc.Calculate(13.37);

            Assert.Equal("f(13.37)=3592.51", result);
        }

        [Theory]
        [InlineData("x", "x^3-2*x^2+sin(x)", @"f(x)=-2{x}^{2} + {x}^{3} + \sin{x}")]
        [InlineData("t", "(sin(t))^2+(cos(t))^2", @"f(t)=\sin^{2}{t} + \cos^{2}{t}")]
        public void TestConvertFunction(string var, string function, string output){
            FuncCalculator calc = new FuncCalculator(var, function);
            Assert.Equal(output, calc.ConvertElement());
        }
    }
}
