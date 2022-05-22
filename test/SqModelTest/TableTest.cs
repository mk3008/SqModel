using SqModel;
using Xunit;

namespace SqModelTest;

public class TableTest
{
    [Fact]
    public void SelectAll()
    {
        var t = new Table() { TableName = "table_a" };

        var text = t.ToQuery().CommandText;
        var expect = @"select table_a.* from table_a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Alias()
    {
        var t = new Table() { TableName = "table_a", AliasName = "a" };

        var text = t.ToQuery().CommandText;
        var expect = @"select a.* from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void AddClolumns()
    {
        var t = new Table() { TableName = "table_a", AliasName = "a" };
        t.AddColumn("column_b");
        t.AddColumn("column_c").AliasName = "c";
        t.AddVirtualColumn("1", "d");
        t.AddVirtualColumn(":val1 + :val2", "e").AddParameter(":val1", 1).AddParameter(":val2", 2);

        var q = t.ToQuery();
        var text = q.CommandText;
        var expect = @"select a.column_b, a.column_c as c, 1 as d, :val1 + :val2 as e from table_a as a";

        Assert.Equal(expect, text);

        Assert.Equal(1, q.Parameters[":val1"]);
        Assert.Equal(2, q.Parameters[":val2"]);
    }

/*    [Fact]
    public void Test1()
    {
        var text = string.Empty;

        var t = new Table()
        {
            TableName = "table_a",
            AliasName = "a",
        };
        //t.AddColumn("*");

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
    }*/
}
