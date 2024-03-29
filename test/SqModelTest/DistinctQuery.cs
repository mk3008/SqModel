using SqModel;
using Xunit;

namespace SqModelTest;

public class DistinctQuery
{
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a").As("a");
        q.Select(table_a, "name");
        q.Select.IsDistinct = true;
        q.Distinct();

        var text = q.ToQuery().CommandText;
        var expect = @"select distinct
    a.name
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Ignore()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a").As("a");
        q.Select(table_a, "name");
        q.Select.IsDistinct = true;
        q.Distinct(false);

        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.name
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SubQuery()
    {
        var q = new SelectQuery();
        var x = q.From(sq =>
        {
            var a = sq.From("table_a").As("a");
            sq.Select(a, "name");
            sq.Distinct();
        }).As("x");
        q.Select(x, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"select
    x.*
from (
    select distinct
        a.name
    from table_a as a
) as x";

        Assert.Equal(expect, text);
    }
}
