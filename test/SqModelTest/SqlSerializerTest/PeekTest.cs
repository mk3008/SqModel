using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class PeekTest
{
    [Fact]
    public void Peek()
    {
        using var p = new Parser("abcd");
        var c = p.Peek();
        Assert.Equal('a', c);
        c = p.Peek();
        Assert.Equal('a', c);
    }

    [Fact]
    public void Eof()
    {
        Assert.Throws<EndOfStreamException>(() =>
        {
            using var p = new Parser("");
            var c = p.Peek();
        });
    }
}
