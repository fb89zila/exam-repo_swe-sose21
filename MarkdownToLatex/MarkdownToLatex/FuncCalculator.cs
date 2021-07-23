using System;
using System.Text.RegularExpressions;
using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using MathNet.Symbolics;
using System.Globalization;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace MarkdownToLatex {
    /// <summary>This class is used for processing single variable functions.</summary>
    public class FuncCalculator : Calculator<string> {

        /// <summary>Variable <see cref="Calculator.Var"/> as SymbolicExpression</summary>
        private Expr varExpr;

        /// <summary>Combined NumberStyles used to parse doubles.</summary>
        private NumberStyles doubleNumStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        /// <summary>Function compiled in <see cref="FuncCalculator()"/> for MathNet calculations.</summary>
        private Func<double, double> func;

        /// <summary>First Derivative Function of <see cref="func"/>.</summary>
        private Func<double, double> dfunc;

        /// <summary>Second Derivative Function of <see cref="func"/>.</summary>
        private Func<double, double> ddfunc;

        //methods
        /// <summary>Rounds <paramref name="tempResult"/> with given <paramref name="precision"/>.</summary>
        /// <returns>Rounded result</returns>
        private double roundResult(double tempResult, int precision)
        {
            if (precision < 0) {
                return Math.Round(tempResult);
            } else if (precision > 15) {
                return Math.Round(tempResult, 15);
            } else {
                return Math.Round(tempResult, precision);
            }
        }

        /// <summary>Calculates the function at the given <paramref name="input"/> rounded with given <paramref name="precision"/>.</summary>
        /// <param name="param">Contains input and precision to calculate the function</param>
        /// <returns>Result in LaTeX format.</returns>
        public string Calculate(Match param)
        {
            double input;
            bool hasInput = Double.TryParse(param.Groups[2].Value, doubleNumStyles, CultureInfo.InvariantCulture, out input);

            if (hasInput) {
                int precision;
                bool hasPrecision = int.TryParse(param.Groups[3].Value, out precision);

                return $"f({(input).ToString(CultureInfo.InvariantCulture)})=" + roundResult(func(input), hasPrecision ? precision : 2).ToString(CultureInfo.InvariantCulture);
            } else {
                throw new ConvertElementException("Input could not be parsed.");
            }
        }

        /// <summary>Tries to find a root in given bounds rounded with given <paramref name="precision"/>.</summary>
        /// <param name="param">Contains bounds as string and precision to calculate a root</param>
        /// <returns>Result in LaTeX format.</returns>
        public string CalcRoot(Match param)
        {
            string[] bounds = param.Groups[2].Value.Split(",");
            
            double lowerBound;
            double upperBound;

            bool hasBounds = Double.TryParse(bounds[0], doubleNumStyles, CultureInfo.InvariantCulture, out lowerBound);
            hasBounds &= Double.TryParse(bounds[1], doubleNumStyles, CultureInfo.InvariantCulture, out upperBound);

            if (hasBounds) {
                int precision;
                bool hasPrecision = int.TryParse(param.Groups[3].Value, out precision);

                double result;
                bool foundRoot = RobustNewtonRaphson.TryFindRoot(func, dfunc, lowerBound, upperBound, 1e-8, 100, 20, out result);
                
                return $"root([{lowerBound},{upperBound}])=" + roundResult(result, hasPrecision ? precision : 2).ToString(CultureInfo.InvariantCulture);
            } else {
                throw new ConvertElementException("Input could not be parsed.");
            }
        }

        /// <summary>Compute the first or second derivative or calculate it with eventual input from <paramref name="param"/>.</summary>
        /// <param name="param">Contains input and precision in case derivative is to be calculated</param><param name="order">Order of derivative</param>
        /// <returns>Derivative or result of calculated derivative in LaTeX format.</returns>
        public string CalcDerivative(Match param, int order)
        {
            double input;
            bool hasInput = Double.TryParse(param.Groups[2].Value, doubleNumStyles, CultureInfo.InvariantCulture, out input);

            int precision;
            bool hasPrecision = int.TryParse(param.Groups[3].Value, out precision);

            switch (order) {
                case 1:
                    if (hasInput) {
                        return $"f'({(input).ToString(CultureInfo.InvariantCulture)})=" + roundResult(dfunc(input), hasPrecision ? precision : 2).ToString(CultureInfo.InvariantCulture);
                    } else {
                        return $"f'({this.Var})=" + Element.Differentiate(varExpr).RationalSimplify(varExpr).ToLaTeX();
                    }
                case 2:
                    if (hasInput) {
                        return $"f''({(input).ToString(CultureInfo.InvariantCulture)})=" + roundResult(ddfunc(input), hasPrecision ? precision : 2).ToString(CultureInfo.InvariantCulture);
                    } else {
                        return $"f''({this.Var})=" + Element.Differentiate(varExpr).Differentiate(varExpr).RationalSimplify(varExpr).ToLaTeX();
                    }
                default:
                    throw new ConvertElementException("Derivatives higher than the second order are not supported."); //Can't be reached, but is needed because else "not every path returns a value"
            }
        }

        /// <summary>Converts the function into LaTeX.</summary>
        /// <returns>The function in LaTeX format.</returns>
        public override string ConvertElement()
        {
            return $"f({this.Var})=" + this.Element.ToLaTeX();
        }

        /// <summary>Initializes a new Instance of the the <see cref="FuncCalculator"/> class.</summary>
        public FuncCalculator(string var, string element) : base(var, element)
        {
            try {
                this.varExpr = Expr.Variable(Var);
            } catch (Exception e) {
                throw new ConvertElementException($"Could not compile function: {e.Message}", e);
            }

            try {
                this.func = this.Element.Compile(this.Var);
            } catch (Exception e) {
                throw new ConvertElementException($"Could not compile function: {e.Message}", e);
            }

            try {
                this.dfunc = Differentiate.FirstDerivativeFunc(func);
                this.ddfunc = Differentiate.SecondDerivativeFunc (func);
            } catch (Exception e) {
                throw new ConvertElementException($"Could not create derivatives: {e.Message}", e);
            }
        }
    }
}