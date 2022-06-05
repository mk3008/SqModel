using SqModel;
using Xunit;

namespace SqModelTest;

public class SelectQueryTest
{
    [Fact]
    public void Default()
    {
        var sq = new SelectQuery();

        var t = new Table() { TableName = "table_a" };
        var ta = new SelectQuery() { Table = t };
        sq.FromClause = ta;

        var text = sq.ToQuery().CommandText;
        var expect = @"select table_a.*
from table_a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Alias()
    {
        var sq = new SelectQuery();

        var t = new Table() { TableName = "table_a", AliasName = "a" };
        var ta = new SelectQuery() { Table = t };

        sq.FromClause = ta;

        var text = sq.ToQuery().CommandText;
        var expect = @"select a.*
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void AliasOverride()
    {
        var sq = new SelectQuery();

        var t = new Table() { TableName = "table_a", AliasName = "a" };
        var ta = new SelectQuery() { Table = t, AliasName = "x" };

        sq.FromClause = ta;

        var text = sq.ToQuery().CommandText;
        var expect = @"select x.*
from table_a as x";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Column()
    {
        var sq = new SelectQuery();

        var t = new Table() { TableName = "table_a", AliasName = "a" };
        t.AddColumn("column_value", "col");
        var ta = new SelectQuery() { Table = t, AliasName = "x" };

        sq.FromClause = ta;

        var text = sq.ToQuery().CommandText;
        var expect = @"select x.*
from (select a.column_value as col from table_a as a) as x";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void ColumnOverride()
    {
        var sq = new SelectQuery();

        var t = new Table() { TableName = "table_a", AliasName = "a" };
        var ta = new SelectQuery() { Table = t, AliasName = "x" };

        sq.FromClause = ta;
        sq.AddColumn(ta, "column_value", "col");

        var text = sq.ToQuery().CommandText;
        var expect = @"select x.column_value as col
from table_a as x";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Columns()
    {
        var sq = new SelectQuery();

        var t = new Table() { TableName = "table_a", AliasName = "a" };
        t.AddColumn("col1", "c1");
        t.AddColumn("col2", "c2");
        var ta = new SelectQuery() { Table = t, AliasName = "x" };

        sq.FromClause = ta;

        var text = sq.ToQuery().CommandText;
        var expect = @"select x.*
from (select a.col1 as c1, a.col2 as c2 from table_a as a) as x";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void ColumnsOverride()
    {
        var sq = new SelectQuery();
        
        var t = new Table() { TableName = "table_a", AliasName = "a" };
        t.AddColumn("col1", "c1");
        t.AddColumn("col2", "c2");
        var ta = new SelectQuery() { Table = t, AliasName = "x" };
                
        sq.FromClause = ta;
        sq.AddColumn(ta, "c1", "value1");
        sq.AddColumn(ta, "c2", "value2");

        var text = sq.ToQuery().CommandText;
        var expect = @"select x.c1 as value1, x.c2 as value2
from (select a.col1 as c1, a.col2 as c2 from table_a as a) as x";

        Assert.Equal(expect, text);
    }
}
