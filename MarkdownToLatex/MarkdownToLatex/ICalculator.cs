namespace MarkdownToLatex
{
    public interface ICalculator
    {
         /// <summary>Converts the <see cref="Element"/> to LaTeX.</summary>
        public string ConvertElement();
    }
}