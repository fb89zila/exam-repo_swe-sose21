using System;
using MathNet.Symbolics;
using System.Globalization;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace MarkdownToLatex {
    /// <summary>This class is used for processing single variable functions.</summary>
    public class FuncCalculator : SingleVarCalculator {

        /// <summary>Takes the <paramref name="var"/> to use and the <paramref name="function"/> to process. Converts the function to LaTeX.</summary>
        /// <returns>a string containing the function converted to LaTeX.</returns>
        public override string ConvertElement(string var, string function)
        {
            this.Element = function;
            this.Variable = char.Parse(var.Substring(0, 1));
            try {
                Expr func = Expr.Parse(function);
                return $"f({var}) = " + func.ToLaTeX();
            } catch (Exception ex) {
                Console.WriteLine("[ERROR] Error converting function: {0}", ex.Message);
                return null;
            }
        }

        /// <summary>Takes the <paramref name="var"/> to use and the <paramref name="function"/> to process. Evaluates the function at <paramref name="param"/> and converts the function to LaTeX.</summary>
        /// <returns>a string containing the function converted to LaTeX.</returns>
        public override string ConvertElement(double param, string var, string function)
        {
            this.Element = function;
            this.Variable = char.Parse(var.Substring(0, 1));
            this.Input = param;
            try {
                Expr func = Expr.Parse(function);
                return $"f({var}) = " + func.ToLaTeX() + $" with f({param.ToString(CultureInfo.InvariantCulture)}) = " + func.Compile(var)(param).ToString(CultureInfo.InvariantCulture);
            } catch (Exception ex){
                Console.WriteLine("[ERROR] Error converting function: {0}", ex.Message);
                return null;
            }
        }
    }
}