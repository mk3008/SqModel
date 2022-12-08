using SqModel.Analysis;
using SqModel.Analysis.Parser;
using SqModel.Core;
using SqModel.Core.Clauses;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class ValueParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public ValueParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Column()
    {
        var text = "col";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("col", v.GetCommandText());
        Assert.Equal("col", v.GetDefaultName());
    }

    [Fact]
    public void TableColumn()
    {
        var text = "tbl.col";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("tbl.col", v.GetCommandText());
        Assert.Equal("col", v.GetDefaultName());
    }

    [Fact]
    public void Numeric()
    {
        var text = "3.14";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("3.14", v.GetCommandText());
    }

    [Fact]
    public void Text()
    {
        var text = "'abc''s'";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("'abc''s'", v.GetCommandText());
    }

    [Fact]
    public void BooleanTrue()
    {
        var text = "true";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("true", v.GetCommandText());
    }

    [Fact]
    public void BooleanFalse()
    {
        var text = "false";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("false", v.GetCommandText());
    }

    [Fact]
    public void Expression()
    {
        var text = "1*3.14";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("1 * 3.14", v.GetCommandText());
    }

    [Fact]
    public void Expression2()
    {
        var text = "tbl.col1 *   tbl.col2";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("tbl.col1 * tbl.col2", v.GetCommandText());
        Assert.Equal("", v.GetDefaultName());
    }

    [Fact]
    public void Expression3()
    {
        var text = "(1+1)*2";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("(1 + 1) * 2", v.GetCommandText());
    }

    [Fact]
    public void Not()
    {
        var text = "not true";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("not true", v.GetCommandText());
    }

    [Fact]
    public void Not2()
    {
        var text = "not (1 + 1 = 1)";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("not (1 + 1 = 1)", v.GetCommandText());
    }

    [Fact]
    public void Function()
    {
        var text = "sum(tbl.col+    tbl.col2)";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("sum(tbl.col + tbl.col2)", v.GetCommandText());
    }

    [Fact]
    public void WindowFunction()
    {
        var text = "row_number() over(partition by tbl.col, tbl.col2 order by tbl.col3, tbl.col4)";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("row_number() over(partition by tbl.col, tbl.col2 order by tbl.col3, tbl.col4)", v.GetCommandText());
    }

    [Fact]
    public void WindowFunction2()
    {
        var text = "row_number() over(order by tbl.col, tbl.col2)";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("row_number() over(order by tbl.col, tbl.col2)", v.GetCommandText());
    }

    [Fact]
    public void CaseExpression()
    {
        var text = "case tbl.col when 1 then 10 when 2 then 20 else 30 end";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("case tbl.col when 1 then 10 when 2 then 20 else 30 end", v.GetCommandText());
    }

    [Fact]
    public void CaseWhenExpression()
    {
        var text = "case when tbl.col1 = 1 then 10 when tbl.col2 = 2 then 20 else 30 end";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.Equal("case when tbl.col1 = 1 then 10 when tbl.col2 = 2 then 20 else 30 end", v.GetCommandText());
    }

    [Fact]
    public void InlineQuery()
    {
        var text = "(select a.val from table_a a where a.id = b.table_a_id)";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.IsType<InlineQuery>(v);
    }

    [Fact]
    public void ExistsExpression()
    {
        var text = "exists (select * from table_a a where a.id = b.table_a_id)";
        var v = ValueParser.Parse(text);
        Monitor.Log(v);

        Assert.IsType<ExistsExpression>(v);
    }
}