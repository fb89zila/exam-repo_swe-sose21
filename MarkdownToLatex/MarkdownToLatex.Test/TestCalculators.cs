using Xunit;

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
    }
}
