#nullable enable
using System;

namespace MarkdownToLatex
{

    /// <summary>The basic calculator class. 
    /// <typeparamref name="T"/> specifies the variable/s to use.</summary>
    public abstract class Calculator<T> : ICalculator {

        /// <summary>The element to process.</summary>
        public string Element {get; protected set;}

        /// <summary>The variable to use.</summary>
        public T Var {get; protected set;}

        /// <summary>Converts the <see cref="Element"/> to LaTeX.</summary>
        public abstract string ConvertElement();

        /// <summary>Initializes a new Calculator. Only used in derived classes.</summary>
        /// <param name="var">The variable/s to use.</param>
        /// <param name="element">The element to process</param>
        /// <param name="param">The parameter/s to use, can be <c>null</c>.</param>
        protected Calculator(T var, string element){
            this.Var = var;
            this.Element = element;
        }
    }
}
