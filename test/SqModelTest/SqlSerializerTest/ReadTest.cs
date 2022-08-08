using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class ReadTest
{
    [Fact]
    public void Read()
    {
        using var p = new Parser("abcd");
        var c = p.Read();
        Assert.Equal('a', c);
        c = p.Read();
        Assert.Equal('b', c);
    }

    [Fact]
    public void Eof()
    {
        Assert.Throws<EndOfStreamException>(() =>
        {
            using var p = new Parser("");
            var c = p.Read();
        });
    }
}