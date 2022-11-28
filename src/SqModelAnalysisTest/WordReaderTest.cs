using Microsoft.VisualStudio.TestPlatform.Utilities;
using SqModel.Analysis;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class WordReaderTest
{

    private readonly ITestOutputHelper Output;

    public WordReaderTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private void LogOutput(List<string> arguments)
    {
        foreach (var item in arguments)
        {
            Output.WriteLine(item);
        }
    }

    [Fact]
    public void Blank()
    {
        var text = "";
        using var r = new WordReader(text);

        foreach (var item in r.ReadWords())
        {
            throw new Exception();
        }
    }

    [Fact]
    public void Colon()
    {
        var text = ":val val::text";
        using var r = new WordReader(text);
        var lst = r.ReadWords().ToList();
        LogOutput(lst);

        Assert.Equal(2, lst.Count);
        Assert.Equal(":val", lst[0]);
        Assert.Equal("val::text", lst[1]);
    }

    [Fact]
    public void Numeric()
    {
        var text = "123 1.23";
        using var r = new WordReader(text);
        var lst = r.ReadWords().ToList();
        LogOutput(lst);

        Assert.Equal(2, lst.Count);
        Assert.Equal("123", lst[0]);
        Assert.Equal("1.23", lst[1]);
    }

    [Fact]
    public void TableColumn()
    {
        var text = "tbl.col1 tbl.col2";
        using var r = new WordReader(text);
        var lst = r.ReadWords().ToList();
        LogOutput(lst);

        Assert.Equal(6, lst.Count);
        Assert.Equal("tbl", lst[0]);
        Assert.Equal(".", lst[1]);
        Assert.Equal("col1", lst[2]);
        Assert.Equal("tbl", lst[3]);
        Assert.Equal(".", lst[4]);
        Assert.Equal("col2", lst[5]);
    }

    [Fact]
    public void SingleQupte()
    {
        var text = "'a b' '   '";
        using var r = new WordReader(text);
        var lst = r.ReadWords().ToList();
        LogOutput(lst);

        Assert.Equal(2, lst.Count);
        Assert.Equal("'a b'", lst[0]);
        Assert.Equal("'   '", lst[1]);
    }

    [Fact]
    public void Operator()
    {
        var text = "1+1!=3";
        using var r = new WordReader(text);
        var lst = r.ReadWords().ToList();
        LogOutput(lst);

        Assert.Equal(5, lst.Count);
        Assert.Equal("1", lst[0]);
        Assert.Equal("+", lst[1]);
        Assert.Equal("1", lst[2]);
        Assert.Equal("!=", lst[3]);
        Assert.Equal("3", lst[4]);
    }

    [Fact]
    public void Pipe()
    {
        var text = "'a' || 'b'";
        using var r = new WordReader(text);
        var lst = r.ReadWords().ToList();
        LogOutput(lst);

        Assert.Equal(3, lst.Count);
        Assert.Equal("'a'", lst[0]);
        Assert.Equal("||", lst[1]);
        Assert.Equal("'b'", lst[2]);
    }

    [Fact]
    public void LineComment()
    {
        var text = "a---b";
        using var r = new WordReader(text);
        var lst = r.ReadWords().ToList();
        LogOutput(lst);

        Assert.Equal(4, lst.Count);
        Assert.Equal("a", lst[0]);
        Assert.Equal("--", lst[1]);
        Assert.Equal("-", lst[2]);
        Assert.Equal("b", lst[3]);
    }

    [Fact]
    public void BlockComment()
    {
        var text = "a/*b*/";
        using var r = new WordReader(text);
        var lst = r.ReadWords().ToList();
        LogOutput(lst);

        Assert.Equal(4, lst.Count);
        Assert.Equal("a", lst[0]);
        Assert.Equal("/*", lst[1]);
        Assert.Equal("b", lst[2]);
        Assert.Equal("*/", lst[3]);
    }

    [Fact]
    public void Function()
    {
        var text = "sum(a.price)";
        using var r = new WordReader(text);
        var lst = r.ReadWords().ToList();
        LogOutput(lst);

        Assert.Equal(6, lst.Count);
        Assert.Equal("sum", lst[0]);
        Assert.Equal("(", lst[1]);
        Assert.Equal("a", lst[2]);
        Assert.Equal(".", lst[3]);
        Assert.Equal("price", lst[4]);
        Assert.Equal(")", lst[5]);
    }
}