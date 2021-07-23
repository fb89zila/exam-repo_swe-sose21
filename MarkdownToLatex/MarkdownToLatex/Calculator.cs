using System;
using Expr = MathNet.Symbolics.SymbolicExpression;

namespace MarkdownToLatex
{

    /// <summary>The basic calculator class. 
    /// <typeparamref name="T"/> specifies the variable/s to use.</summary>
    public abstract class Calculator<T> : ICalculator {

        /// <summary>The element to process.</summary>
        public Expr Element {get;}

        /// <summary>The variable to use.</summary>
        public T Var {get;}

        /// <summary>Converts the <see cref="Element"/> to LaTeX.</summary>
        public abstract string ConvertElement();

        /// <summary>Initializes a new Calculator and parses math element string.  
        /// Only used in derived classes.</summary>
        /// <param name="var">The variable/s to use.</param>
        /// <param name="element">The element to process</param>
        protected Calculator(T var, string element)
        {
            this.Var = var;

            try {
                this.Element = Expr.Parse(element);
            } catch (Exception e) {
                throw new ConvertElementException($"Could not parse function: {e.Message}", e);
            }
        }
    }
}
