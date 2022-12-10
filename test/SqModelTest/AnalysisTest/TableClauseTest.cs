using SqModel;
using SqModel.Analysis;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class TableClauseTest
{
    public TableClauseTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    private string ToString(List<TableClause> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("| Type | Table | Alias | Condition |");
        sb.AppendLine("| --- | ------- | ---- | --------- |");

        foreach (var item in items)
        {
            sb.AppendLine($"| {item.RelationType} | {item.TableName} | {item.AliasName} | {item.RelationClause.ToQuery().CommandText} |");
        }
        Output.WriteLine(sb.ToString());
        return sb.ToString();
    }

    [Fact]
    public void TableInfo_Parse()
    {
        var sq = SqlParser.Parse(@"select 
    *
from
    a
    inner join table_b as b on a.table_a_id = b.table_a_id
    left join table_c as c1 on b.table_b_id = c1.table_b_id
    right join table_c as c2 on b.table_b_id = c2.table_b_id
    inner join (select * from e) as z on a.table_a_id = z.table_a_id");

        var items = sq.GetTableClauses();
        var s = ToString(items);

        var expect = @"| Type | Table | Alias | Condition |
| --- | ------- | ---- | --------- |
| From | a | a |  |
| Inner | table_b | b | a.table_a_id = b.table_a_id |
| Left | table_c | c1 | b.table_b_id = c1.table_b_id |
| Right | table_c | c2 | b.table_b_id = c2.table_b_id |
| Inner |  | z | a.table_a_id = z.table_a_id |
";

        Assert.Equal(expect, s);

        Assert.Equal(5, sq.GetTableClauses().Count);

        Assert.True(sq.TableAliasContains("c1"));
        var item = sq.GetTableClauseByName("c1");
        Assert.Equal("c1", item.AliasName);
        Assert.Equal("table_c", item.TableName);

        Assert.False(sq.TableAliasContains("table_1"));

        Assert.Equal(2, sq.GetTableClausesByTable("table_c").Count);
    }

}
