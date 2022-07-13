using SqModel;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class ParenthesesTest
{
    [Fact]
    public void Default()
    {
        using var p = new Parser("(test)");

        var q = p.ParseParenthese();

        var lst = q.GetValues().ToList();

        Assert.Equal(3, lst.Count());
        Assert.Equal("(", lst[0]);
        Assert.Equal("test", lst[1]);
        Assert.Equal(")", lst[2]);
    }

    [Fact]
    public void Nest()
    {
        using var p = new Parser("(test1(test2)test3)");

        var q = p.ParseParenthese();

        var lst = q.GetValues().ToList();

        Assert.Equal(7, lst.Count());
        Assert.Equal("(", lst[0]);
        Assert.Equal("test1", lst[1]);
        Assert.Equal("(", lst[2]);
        Assert.Equal("test2", lst[3]);
        Assert.Equal(")", lst[4]);
        Assert.Equal("test3", lst[5]);
        Assert.Equal(")", lst[6]);
    }
}
