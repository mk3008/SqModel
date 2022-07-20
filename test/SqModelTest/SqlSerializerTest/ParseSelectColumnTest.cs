using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ParseSelectColumnTest
{
    public ParseSelectColumnTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        using var p = new SelectQueryParser(@"
    a.column_1 as col1
    , a.column_2 col2
    , a.column_3
    , column_11 as col11
    , column_12 col12
    , column_13");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);
        Assert.Equal(6, s.Columns.Count);

        Assert.Equal("a", s.Columns[0].TableName);
        Assert.Equal("column_1", s.Columns[0].ColumnName);
        Assert.Equal("col1", s.Columns[0].AliasName);

        Assert.Equal("a", s.Columns[1].TableName);
        Assert.Equal("column_2", s.Columns[1].ColumnName);
        Assert.Equal("col2", s.Columns[1].AliasName);

        Assert.Equal("a", s.Columns[2].TableName);
        Assert.Equal("column_3", s.Columns[2].ColumnName);
        Assert.Equal("column_3", s.Columns[2].AliasName);

        Assert.Equal("", s.Columns[3].TableName);
        Assert.Equal("column_11", s.Columns[3].ColumnName);
        Assert.Equal("col11", s.Columns[3].AliasName);

        Assert.Equal("", s.Columns[4].TableName);
        Assert.Equal("column_12", s.Columns[4].ColumnName);
        Assert.Equal("col12", s.Columns[4].AliasName);

        Assert.Equal("", s.Columns[5].TableName);
        Assert.Equal("column_13", s.Columns[5].ColumnName);
        Assert.Equal("column_13", s.Columns[5].AliasName);
    }

    [Fact]
    public void Full()
    {
        using var p = new SelectQueryParser("a.column_1 as col1");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);

        Assert.Single(s.Columns);
        Assert.Equal("a", s.Columns[0].TableName);
        Assert.Equal("column_1", s.Columns[0].ColumnName);
        Assert.Equal("col1", s.Columns[0].AliasName);
    }

    [Fact]
    public void OmitTable()
    {
        using var p = new SelectQueryParser("column_1 as col1");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);

        Assert.Single(s.Columns);
        Assert.Equal("", s.Columns[0].TableName);
        Assert.Equal("column_1", s.Columns[0].ColumnName);
        Assert.Equal("col1", s.Columns[0].AliasName);
    }

    [Fact]
    public void OmitAliasToken()
    {
        using var p = new SelectQueryParser("a.column_1 col1");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);

        Assert.Single(s.Columns);
        Assert.Equal("a", s.Columns[0].TableName);
        Assert.Equal("column_1", s.Columns[0].ColumnName);
        Assert.Equal("col1", s.Columns[0].AliasName);
    }

    [Fact]
    public void Brief()
    {
        using var p = new SelectQueryParser("column_1");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);

        Assert.Single(s.Columns);
        Assert.Equal("", s.Columns[0].TableName);
        Assert.Equal("column_1", s.Columns[0].ColumnName);
        Assert.Equal("column_1", s.Columns[0].AliasName);
    }
}
