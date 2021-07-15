namespace MarkdownToLatex {

    /// <summary>The base class for all calculators with only one variable. Contains properties for the variable and the input.</summary>
    public abstract class SingleVarCalculator : Calculator {

        /// <summary>The variable to use.</summary>
        public char Variable {get; protected set;}

        /// <summary>The input element to process.</summary>
        public double Input {get; protected set;}

        /// <summary>Takes the <paramref name="var"/> to use and an <paramref name="element"/> to process. Converts the <paramref name="element"/> to LaTeX.</summary>
        /// <returns>a string containing the element converted to LaTeX.</returns>
        public abstract string ConvertElement(string var, string element);

        /// <summary>Takes the <paramref name="var"/> to use, an <paramref name="element"/> to process and a <paramref name="param"/>. Converts the <paramref name="element"/> to LaTeX and evaluates it at the <paramref name="param"/>.</summary>
        /// <returns>a string containing the evaluated element converted to LaTeX.</returns>
        public abstract string ConvertElement(double param, string var, string element);
    }
}