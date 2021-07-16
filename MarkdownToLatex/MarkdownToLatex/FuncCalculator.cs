using System;
using MathNet.Symbolics;
using System.Globalization;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace MarkdownToLatex {
    /// <summary>This class is used for processing single variable functions.</summary>
    public class FuncCalculator : Calculator<double, string> {

        /// <summary>Calculates the function at the given parameter, if no parameter is set, assuming '0'</summary>
        public string Calculate(){
            try{
                var func = Expr.Parse(this.Element);
                return $"f({this.Var})=" + Math.Round(func.Compile(this.Var)(this.Param ?? 0), 2).ToString(CultureInfo.InvariantCulture);
            } catch (Exception ex){
                throw new ConvertElementException("Error calculating function!", ex);
            }
        }

        /// <summary>Converts the function into LaTeX.</summary>
        /// <returns>The function in LaTex format.</returns>
        public override string ConvertElement(){
            try{
                var func = Expr.Parse(this.Element);
                return $"f({this.Var})=" + func.ToLaTeX();
            } catch (Exception ex){
                throw new ConvertElementException("Error converting function!", ex);
            }
        }

        /// <summary>Initializes a new Instance of the the <see cref="FuncCalculator"/> class.</summary>
        public FuncCalculator(string var, string element, double? param = null) : base(var, element, param){}
    }
}