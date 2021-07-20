using System;
using MathNet.Symbolics;
using System.Globalization;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace MarkdownToLatex {
    /// <summary>This class is used for processing single variable functions.</summary>
    public class FuncCalculator : Calculator<string> {

        /// <summary>Calculates the function at the given <paramref name="param"/>.
        /// The <paramref name="precision"/> paameter specifies the rounding accuracy.</summary>
        public string Calculate(double param, int precision = 2){
            try{
                var func = Expr.Parse(this.Element);
                return $"f({(param).ToString(CultureInfo.InvariantCulture)})=" + Math.Round(func.Compile(this.Var)(param), precision).ToString(CultureInfo.InvariantCulture);
            } catch (Exception ex){
                throw new ConvertElementException($"Error calculating function: {ex.Message}", ex);
            }
        }

        /// <summary>Converts the function into LaTeX.</summary>
        /// <returns>The function in LaTex format.</returns>
        public override string ConvertElement(){
            try{
                var func = Expr.Parse(this.Element);
                return $"f({this.Var})=" + func.ToLaTeX();
            } catch (Exception ex){
                throw new ConvertElementException($"Error converting function: {ex.Message}", ex);
            }
        }

        /// <summary>Initializes a new Instance of the the <see cref="FuncCalculator"/> class.</summary>
        public FuncCalculator(string var, string element) : base(var, element){}
    }
}