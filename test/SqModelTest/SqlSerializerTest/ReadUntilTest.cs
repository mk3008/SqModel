using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class ReadUntilTest
{
    [Fact]
    public void FindFirst()
    {
        using var p = new Parser("abcd");
        var s = p.ReadUntil("a");
        Assert.Equal("", s);
        Assert.Equal('a', p.Peek());
    }

    [Fact]
    public void FindChar()
    {
        using var p = new Parser("abcd");
        var s = p.ReadUntil("b");
        Assert.Equal("a", s);
        Assert.Equal('b', p.Peek());
    }

    [Fact]
    public void FindString()
    {
        using var p = new Parser("abcd");
        var s = p.ReadUntil("c");
        Assert.Equal("ab", s);
        Assert.Equal('c', p.Peek());
    }

    [Fact]
    public void IgnoreCase()
    {
        using var p = new Parser("abcd");
        var s = p.ReadUntil("C");
        Assert.Equal("ab", s);
        Assert.Equal('c', p.Peek());
    }

    [Fact]
    public void NoCase()
    {
        using var p = new Parser("123456789");
        var s = p.ReadUntil("5");
        Assert.Equal("1234", s);
        Assert.Equal('5', p.Peek());
    }

    [Fact]
    public void NotFound()
    {
        using var p = new Parser("abcd");
        var s = p.ReadUntil("z");
        Assert.Equal("abcd", s);
    }

    [Fact]
    public void MulitpleKeyword()
    {
        var chars = "cd"; //c or d
        using var p1 = new Parser("abc");
        var s = p1.ReadUntil(chars);
        Assert.Equal("ab", s);
        Assert.Equal('c', p1.Peek());

        using var p2 = new Parser("abd");
        s = p2.ReadUntil(chars);
        Assert.Equal("ab", s);
        Assert.Equal('d', p2.Peek());
    }
}
