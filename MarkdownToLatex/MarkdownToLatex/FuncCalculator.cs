using System;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace MarkdownToLatex {
    /// <summary>
    /// This class is used for processing single variable functions.
    /// </summary>
    public class FuncCalculator : SingleVarCalculator {

        /// <summary>
        /// Takes the variable to use and the function to process.
        /// Converts the function to LaTeX.
        /// </summary>
        /// <returns>
        /// a string containing the function converted to LaTeX.
        /// </returns>
        /// <param name="vars">The variable to use.</param>
        /// <param name="function">The function to process.</param>
        public override string ConvertElement(string vars, string function)
        {
            try {
                Expr func = Expr.Parse(function);
                return func.ToLaTeX();
            } catch (Exception ex) {
                Console.WriteLine("[ERROR] Error converting function: {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Takes the variable to use and the function to process.
        /// Converts the function to LaTeX.
        /// </summary>
        /// <returns>
        /// a string containing the function converted to LaTeX.
        /// </returns>
        /// <param name="vars">The variable to use.</param>
        /// <param name="function">The function to process.</param>
        /// <param name="param">The parameter at which the function should be evaluated.</param>
        public override string ConvertElement(double param, string vars, string function)
        {
            try {
                Expr func = Expr.Parse(function);
                return func.ToLaTeX() + " = " + func.Compile(vars)(param);
            } catch (Exception ex){
                Console.WriteLine("[ERROR] Error converting function: {0}", ex.Message);
                return null;
            }
        }
    }
}