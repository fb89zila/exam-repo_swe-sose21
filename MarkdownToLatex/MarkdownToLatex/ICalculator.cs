namespace MarkdownToLatex
{
    /// <summary>Interface for Calculator (used to enable polymorphism in <see cref="MdToTex"/>)</summary>
    public interface ICalculator
    {
         /// <summary>Converts the <see cref="Element"/> to LaTeX.</summary>
        public string ConvertElement();
    }
}