using System;
using MathNet.Symbolics;
using System.Globalization;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace MarkdownToLatex {
    /// <summary>This class is used for processing single variable functions.</summary>
    public class FuncCalculator : Calculator<double, string> {

        public double Calculate(){
            try{
                var func = Expr.Parse(this.Element);
                return func.Compile(this.Var)(this.Param ?? 0);
            } catch (Exception ex){
                throw new ConvertElementException("Error calculating function!", ex);
            }
        }

        public override string ConvertElement(){
            try{
                var func = Expr.Parse(this.Element);
                return func.ToLaTeX();
            } catch (Exception ex){
                throw new ConvertElementException("Error converting function!", ex);
            }
        }

        public FuncCalculator(string var, string element, double? param = null) : base(var, element, param){}
    }
}