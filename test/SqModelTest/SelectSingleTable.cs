using SqModel;
using Xunit;

namespace SqModelTest;

public class SelectSingleTable
{
    [Fact]
    public void SelectAll()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"select table_a.*
from table_a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void TableNameAlias()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(table_a, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.*
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectColumn()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(table_a, "column_x");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.column_x
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectColumnWithAlias()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(table_a, "column_x", "x");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.column_x as x
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectStaticValue()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select("'test'", "test");

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select 'test' as test
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectVariable()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(":val", "value").AddParameter(":val", 1);

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select :val as value
from table_a as a";

        Assert.Equal(expect, text);
        Assert.Equal(1, actual.Parameters[":val"]);
    }

    [Fact]
    public void SelectColumns()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");

        q.Select(table_a, "column_x", "x");
        q.Select(":val", "value").AddParameter(":val", 1);

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select a.column_x as x, :val as value
from table_a as a";

        Assert.Equal(expect, text);
        Assert.Equal(1, actual.Parameters[":val"]);
    }
}
