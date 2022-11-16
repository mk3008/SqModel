using SqModel;
using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class Analyze
{
    public Analyze(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    private string ToString(List<SelectItem> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("| SQL | Command | Name |");
        sb.AppendLine("| --- | ------- | ---- |");

        foreach (var item in items)
        {
            sb.AppendLine($"| {item.ToQuery().CommandText} | {item.ToCommandText()} | {item.Name} |");
        }
        Output.WriteLine(sb.ToString());
        return sb.ToString();
    }

    [Fact]
    public void ColumnInfo_Parse()
    {
        var sq = SqlParser.Parse(@"select 
    a.val1
    , a.val2 as val2
    , a.val3 as value3
    , 1
    , 2 as num2
    , a.*
from
    table as a");

        var items = sq.GetSelectItems();
        var s = ToString(items);

        var expect = @"| SQL | Command | Name |
| --- | ------- | ---- |
| a.val1 | a.val1 | val1 |
| a.val2 | a.val2 | val2 |
| a.val3 as value3 | a.val3 | value3 |
| 1 | 1 |  |
| 2 as num2 | 2 | num2 |
| a.* | a.* |  |
";

        Assert.Equal(expect, s);
    }

    [Fact]
    public void ColumnInfo_Build()
    {
        var sq = new SelectQuery();
        var a = sq.From("table").As("a");

        sq.Select.Add().Column(a, "val1");
        sq.Select.Add().Column(a, "val2").As("val2");
        sq.Select.Add().Column(a, "val3").As("value3");
        sq.Select.Add().Value(1);
        sq.Select.Add().Value(2).As("num2");
        sq.SelectAll(a);

        var items = sq.GetSelectItems();
        var s = ToString(items);

        var expect = @"| SQL | Command | Name |
| --- | ------- | ---- |
| a.val1 | a.val1 | val1 |
| a.val2 | a.val2 | val2 |
| a.val3 as value3 | a.val3 | value3 |
| 1 | 1 |  |
| 2 as num2 | 2 | num2 |
| a.* | a.* |  |
";

        Assert.Equal(expect, s);
    }
}
