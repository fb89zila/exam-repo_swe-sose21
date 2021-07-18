#nullable enable
using System;

namespace MarkdownToLatex
{

    /// <summary>The basic calculator class. 
    /// <typeparamref name="T"/> specifies the parameter/s to use and cannot be a reference type.
    /// <typeparamref name="U"/> specifies the variable/s to use.</summary>
    public abstract class Calculator<T, U> : ICalculator where T : struct //Needed or else Nullable will not work!
    {

        /// <summary>The element to process.</summary>
        public string Element {get; protected set;}

        /// <summary>The parameter to use. Can be <c>null</c>.</summary>
        public T? Param {get; protected set;}

        /// <summary>The variable to use.</summary>
        public U Var {get; protected set;}

        /// <summary>Converts the <see cref="Element"/> to LaTeX.</summary>
        public abstract string ConvertElement();

        /// <summary>Initializes a new Calculator. Only used in derived classes.</summary>
        /// <param name="var">The variable/s to use.</param>
        /// <param name="element">The element to process</param>
        /// <param name="param">The parameter/s to use, can be <c>null</c>.</param>
        protected Calculator(U var, string element, T? param = null){
            this.Var = var;
            this.Element = element;
            this.Param = param;
        }
    }
}
