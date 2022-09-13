using Microsoft.VisualStudio.TestPlatform.Utilities;
using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseAliasTest
{
    public ParseAliasTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        var text = "as col";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseAlias();

        Assert.Equal("col", val);
    }

    [Fact]
    public void Omit()
    {
        var text = "col";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseAlias();

        Assert.Equal("col", val);
    }

    [Fact]
    public void NoAlias()
    {
        var text = ",";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseAlias();

        Assert.Equal("", val);
    }

    [Fact]
    public void ColumnAlias()
    {
        var text = "t.column as col";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseValueClause();
        var sql = val.ToQuery().CommandText;

        Assert.Equal("t.column", sql);
        Assert.Equal("col", p.ParseAlias());
    }

    [Fact]
    public void ColumnAliasOmit()
    {
        var text = "t.column col";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var val = p.ParseValueClause();
        var sql = val.ToQuery().CommandText;

        Assert.Equal("t.column", sql);
        Assert.Equal("col", p.ParseAlias());
    }
}
