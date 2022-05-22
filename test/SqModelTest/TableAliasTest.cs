using SqModel;
using Xunit;

namespace SqModelTest;

public class TableAliasTest
{
    [Fact]
    public void Default()
    {
        var t = new Table() { TableName = "table_a" };
        var ta = new TableAlias() { Table = t };

        var text = ta.ToQuery().CommandText;
        var expect = @"table_a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Alias()
    {
        var t = new Table() { TableName = "table_a" };
        var ta = new TableAlias() { Table = t, AliasName = "x" };

        var text = ta.ToQuery().CommandText;
        var expect = @"table_a as x";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Alias2()
    {
        var t = new Table() { TableName = "table_a", AliasName = "a" };
        var ta = new TableAlias() { Table = t, AliasName = "x" };

        var text = ta.ToQuery().CommandText;
        var expect = @"table_a as x";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectAllColumns()
    {
        var t = new Table() { TableName = "table_a", AliasName = "a" };
        var ta = new TableAlias() { Table = t, AliasName = "x" };

        Assert.Single(ta.GetColumns());

        var q = ta.GetColumns()[0].ToQuery();

        Assert.Equal("x.*", q.CommandText);
    }

    [Fact]
    public void CustomColumns()
    {
        var t = new Table() { TableName = "table_a", AliasName = "a" };
        t.AddColumn("col_b");
        t.AddColumn("col_c", "c");
        t.AddVirtualColumn(":val1", "d");

        var ta = new TableAlias() { Table = t, AliasName = "x" };

        var text = ta.ToQuery().CommandText;
        var expect = @"(select a.col_b, a.col_c as c, :val1 as d from table_a as a) as x";

        Assert.Equal(expect, text);

        var lst = ta.GetColumns();

        Assert.Equal("x.col_b", lst[0].ToQuery().CommandText);
        Assert.Equal("x.c", lst[1].ToQuery().CommandText);
        Assert.Equal("x.d", lst[2].ToQuery().CommandText);
    }
}
