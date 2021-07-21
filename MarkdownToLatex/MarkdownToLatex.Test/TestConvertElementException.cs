using Xunit;
using System;

namespace MarkdownToLatex.Test {


    public class TestConvertElementException{
        [Fact]
        public void TestThrowException(){
            Assert.ThrowsAsync<ConvertElementException>(() => {throw new ConvertElementException();});
            Assert.ThrowsAsync<ConvertElementException>(() => {throw new ConvertElementException("Test");});
            Assert.ThrowsAsync<ConvertElementException>(() => {throw new ConvertElementException("Test", new Exception("inner exception"));});
        }
    }
}