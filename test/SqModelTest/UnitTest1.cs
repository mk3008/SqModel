using SqModel;
using Xunit;

namespace SqModelTest;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var text = string.Empty;

        var t = new Table()
        {
            TableName = "table_a",
            AliasName = "a",
        };
        t.AddColumn("*");

        text = t.ToQuery().CommandText;

        var ta = new TableAlias()
        {
            Table = t,
            AliasName = "x"
        };

        text = ta.ToQuery().CommandText;

        var sq = new SelectQuery()
        {
            RootTable = ta,
            Columns = ta.Columns,
        };

        text = sq.ToQuery().CommandText;
        var expect = @"select x.*
from (select a.* from table_a as a) as x";

        Assert.Equal(expect, text);
    }
}
