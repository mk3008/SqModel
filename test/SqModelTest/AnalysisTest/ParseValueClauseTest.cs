using SqModel.Analysis;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseValueClauseTest
{
    public ParseValueClauseTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        var text = "t.column";
        var val = ValueClauseParser.Parse(text);
        var sql = val.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }

    [Fact]
    public void Value()
    {
        var text = "1";
        var val = ValueClauseParser.Parse(text);
        var sql = val.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }

    [Fact]
    public void Condition()
    {
        var text = "1 = 1";
        var val = ValueClauseParser.Parse(text);
        var sql = val.ToQuery().CommandText;

        Assert.Equal("1 = 1", sql);
    }

    [Fact]
    public void Condition2()
    {
        var text = "1 + 1 = 2";
        var val = ValueClauseParser.Parse(text);
        var sql = val.ToQuery().CommandText;

        Assert.Equal("1 + 1 = 2", sql);
    }

    [Fact]
    public void Expression()
    {
        var text = "(1 + 2) * 3.4";
        var val = ValueClauseParser.Parse(text);
        var sql = val.ToQuery().CommandText;

        Assert.Equal("(1 + 2) * 3.4", sql);
    }

    [Fact]
    public void ExpressionUseColumn()
    {
        var text = "a.value * 3.4";
        var val = ValueClauseParser.Parse(text);
        var sql = val.ToQuery().CommandText;

        Assert.Equal("a.value * 3.4", sql);
    }

    [Fact]
    public void CaseWhenExpression()
    {
        var text = "case when 1 = 1 then 1 else 2 end";
        var val = ValueClauseParser.Parse(text);
        var sql = val.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }

    [Fact]
    public void InlineQuery()
    {
        var text = "(select t.colmn from table as t)";
        var val = ValueClauseParser.Parse(text);
        var sql = val.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }
}
