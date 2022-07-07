using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class ReadSkip
{
    [Fact]
    public void Skip()
    {
        using var p = new Parser("    123abc");
        var i = p.ReadSkipSpaces();
        Assert.Equal('1', (char)i);
        var s = p.Read();
        Assert.Equal('1', s);
    }

    [Fact]
    public void SkipToLast()
    {
        using var p = new Parser("    ");
        var i = p.ReadSkipSpaces();
        Assert.Equal(-1, i);
    }
}
