using SqModel;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class ReadCommandOrDefaultTest
{
    [Fact]
    public void Success()
    {
        var lst = new List<string>() { "select" };
        using var p = new Parser("select * from table");
        var s = p.ReadCommandOrDefault(lst);

        Assert.Equal("select", s);
    }

    [Fact]
    public void Fail()
    {
        var lst = new List<string>() { "from" };
        using var p = new Parser("select * from table");
        var s = p.ReadCommandOrDefault(lst);

        Assert.Equal("", s);
    }

    [Fact]
    public void Fial_shortmatch()
    {
        var lst = new List<string>() { "selection" };
        using var p = new Parser("select * from table");

        var s = p.ReadCommandOrDefault(lst);

        Assert.Equal("", s);
    }

    [Fact]
    public void Fail_longmatch()
    {
        var lst = new List<string>() { "select" };
        using var p = new Parser("selection * from table");
        var s = p.ReadCommandOrDefault(lst);

        Assert.Equal("", s);
    }

    [Fact]
    public void CaseIgnore()
    {
        var lst = new List<string>() { "select" };
        using var p = new Parser("Select * from table");
        var s = p.ReadCommandOrDefault(lst);

        Assert.Equal("select", s);
    }

    [Fact]
    public void MultipleCommand()
    {
        var lst = new List<string>() { "select", "from" };

        using var p1 = new Parser("from table");
        var s = p1.ReadCommandOrDefault(lst);
        Assert.Equal("from", s);

        using var p2 = new Parser("select table");
        s = p2.ReadCommandOrDefault(lst);
        Assert.Equal("select", s);
    }
}
