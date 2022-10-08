using SqModel;
using SqModel.Analysis;
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
        var expect = @"select
    table_a.*
from table_a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void TableNameAlias()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a").As("a");
        q.Select(table_a, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.*
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectColumn()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a").As("a");
        q.Select(table_a, "column_x");

        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.column_x
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectColumnWithAlias()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a").As("a");
        q.Select(table_a, "column_x").As("x");

        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.column_x as x
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectStaticValue()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a").As("a");
        q.Select("'test'").As("test");

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select
    'test' as test
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectVariable()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a").As("a");
        q.Select(":val").As("value").AddParameter(":val", 1);

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select
    :val as value
from table_a as a";

        Assert.Equal(expect, text);
        Assert.Equal(1, actual.Parameters[":val"]);
    }

    [Fact]
    public void SelectColumns()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a").As("a");

        q.Select(table_a, "column_x").As("x");
        q.Select(":val").AddParameter(":val", 1).As("value");

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select
    a.column_x as x
    , :val as value
from table_a as a";

        Assert.Equal(expect, text);
        Assert.Equal(1, actual.Parameters[":val"]);
    }


    [Fact]
    public void ValuesTest()
    {
        var sql = @"select
    *
from (
    values
        (1, 2, 3)
        , (4, 5, 6)
) as v(c1, c2, c3)";
        var sq = SqlParser.Parse(sql);
        var q = sq.ToQuery();

        Assert.Equal(sql, q.CommandText);
    }
}
