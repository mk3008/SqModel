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

public class ReadKeywordOrDefaultTest
{


    [Fact]
    public void Success()
    {
        var lst = new List<string>() { "select" };
        using var p = new Parser("select * from table");
        var s = p.ReadKeywordOrDefault(lst);
        Assert.Equal("select", s.value);
        Assert.Equal("select", s.keyword);
    }

    [Fact]
    public void Fail_start()
    {
        var lst = new List<string>() { "from" };
        using var p = new Parser("select * from table");
        var s = p.ReadKeywordOrDefault(lst);
        Assert.Equal("", s.value);
        Assert.Equal("", s.keyword);
    }

    [Fact]
    public void Fial_shortmatch()
    {
        var lst = new List<string>() { "selection" };
        using var p = new Parser("select * from table");
        var s = p.ReadKeywordOrDefault(lst);
        Assert.Equal("select", s.value);
        Assert.Equal("", s.keyword);
    }

    [Fact]
    public void Fail_longmatch()
    {
        var lst = new List<string>() { "select" };
        using var p = new Parser("selection * from table");
        var s = p.ReadKeywordOrDefault(lst);
        Assert.Equal("select", s.value);
        Assert.Equal("", s.keyword);
    }

    [Fact]
    public void CaseIgnore()
    {
        var lst = new List<string>() { "select" };
        using var p = new Parser("Select * from table");
        var s = p.ReadKeywordOrDefault(lst);
        Assert.Equal("Select", s.value);
        Assert.Equal("select", s.keyword);
    }

    [Fact]
    public void MultipleKeyword()
    {
        var lst = new List<string>() { "SELECT", "FROM" };

        using var p1 = new Parser("from table");
        var s = p1.ReadKeywordOrDefault(lst);
        Assert.Equal("from", s.value);
        Assert.Equal("FROM", s.keyword);

        using var p2 = new Parser("select table");
        s = p2.ReadKeywordOrDefault(lst);
        Assert.Equal("select", s.value);
        Assert.Equal("SELECT", s.keyword);
    }
}
