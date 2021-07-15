using System;

namespace MarkdownToLatex {

    /// <summary>Represents errors that occur during conversion of a math element.</summary>
    public class ConvertElementException : Exception {

        /// <summary>Initializes a new Instance of the <see cref="ConvertElementException"/> class.</summary>
        public ConvertElementException(){}

        /// <summary>Initializes a new Instance of the <see cref="ConvertElementException"/> class.</summary>
        public ConvertElementException(string message) : base(message){}

        /// <summary>Initializes a new Instance of the <see cref="ConvertElementException"/> class.</summary>
        public ConvertElementException(string message, Exception inner) : base(message, inner){}
    }
}