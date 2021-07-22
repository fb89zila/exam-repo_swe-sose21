using System;
using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using MathNet.Symbolics;
using System.Globalization;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace MarkdownToLatex {
    /// <summary>This class is used for processing single variable functions.</summary>
    public class FuncCalculator : Calculator<string> {

        /// <summary>Function compiled in <see cref="FuncCalculator()"> for MathNet calculations.</summary>
        private Func<double, double> func;

        private double roundResult(double tempResult, int precision)
        {
            if (precision <= 0) {
                return Math.Round(tempResult);
            } else if (precision > 15) {
                return Math.Round(tempResult, 15);
            } else {
                return Math.Round(tempResult, precision);
            }
        }

        /// <summary>Calculates the function at the given <paramref name="input"/>.
        /// The <paramref name="precision"/> parameter specifies the rounding accuracy.</summary>
        public string Calculate(double input, int precision)
        {
            double result = roundResult(func(input), precision);

            return $"f({(input).ToString(CultureInfo.InvariantCulture)})=" + result.ToString(CultureInfo.InvariantCulture);
        }

        public string CalcRoot(double lower, double upper, int precision)
        {
            double result = roundResult(Bisection.FindRoot(func, lower, upper), precision);
            
            return $"root([{lower},{upper}])=" + result.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Converts the function into LaTeX.</summary>
        /// <returns>The function in LaTex format.</returns>
        public override string ConvertElement()
        {
            return $"f({this.Var})=" + this.Element.ToLaTeX();
        }

        /// <summary>Initializes a new Instance of the the <see cref="FuncCalculator"/> class.</summary>
        public FuncCalculator(string var, string element) : base(var, element)
        {
            try {
                this.func = this.Element.Compile(this.Var);
            } catch (Exception e) {
                throw new ConvertElementException($"Could not compile function: {e.Message}", e);
            }
        }
    }
}