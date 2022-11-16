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

public class CommonTableTest
{
    public CommonTableTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    private string ToString(List<CommonTable> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("| Name | Query |");
        sb.AppendLine("| ---- | ----- |");

        foreach (var item in items)
        {
            item.Query.IsOneLineFormat = true;
            sb.AppendLine($"| {item.Name} | {item.Query.ToQuery().CommandText} |");
        }
        Output.WriteLine(sb.ToString());
        return sb.ToString();
    }

    [Fact]
    public void CommonTableInfo_Parse()
    {
        var sq = SqlParser.Parse(@"with
a as (
    select id, name from table_a
), 
b as (
    select id, value from table_b
), 
c as (
    select id, text from table_c
)
select
    *
from
    a
    inner join b on a.id = b.id
    inner join c on a.id = c.id
");

        var items = sq.GetCommonTables().ToList();
        var s = ToString(items);

        var expect = @"| Name | Query |
| ---- | ----- |
| a | select id, name from table_a |
| b | select id, value from table_b |
| c | select id, text from table_c |
";

        Assert.Equal(expect, s);

        Assert.Equal(3, sq.GetCommonTables().ToList().Count);

        Assert.True(sq.CommonTableContains("a"));
        var item = sq.GetCommonTable("a");
        Assert.Equal("a", item.Name);
        Assert.Equal("select id, name from table_a", item.Query.ToQuery().CommandText);

        Assert.False(sq.CommonTableContains("z"));
    }
}
