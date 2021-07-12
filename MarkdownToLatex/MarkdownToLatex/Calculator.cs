namespace MarkdownToLatex
{

    /// <summary>
    /// The basic calculator class.
    /// Contains just the element to process.
    /// </summary>
    public abstract class Calculator
    {

        /// <summary>
        /// The element to process.
        /// </summary>
        public string Element {get; private set;}
    }
}
