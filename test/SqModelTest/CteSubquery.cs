using SqModel;
using Xunit;

namespace SqModelTest;

public class CteSubquery
{
    private SelectQuery CreateCommonQuery(string tableName, string aliasName)
    {
        var q = new SelectQuery();
        q.With(aliasName).As(x =>
        {
            var t = x.From(tableName);
            x.Select(t, "*");
        });

        var table = q.From(aliasName);
        q.Select(table, "*");

        return q;
    }

    [Fact]
    public void Default()
    {
        var commonA = CreateCommonQuery("table_a", "a");

        var text = commonA.ToQuery().CommandText;
        var expect = @"with
a as (
    select
        table_a.*
    from table_a
)
select
    a.*
from a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Nest()
    {
        var commonA = CreateCommonQuery("table_a", "a");

        var q = new SelectQuery();
        var y = q.From(commonA).As("y");
        q.Select(y, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"with
a as (
    select
        table_a.*
    from table_a
)
select
    y.*
from (
    select
        a.*
    from a
) as y";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Merge()
    {
        var commonA = CreateCommonQuery("table_a", "a");

        var commonY = new SelectQuery();
        var table_a = commonY.From(commonA).As("a");
        commonY.Select(table_a, "*");

        var text = commonY.ToQuery().CommandText;
        var expect = @"with
a as (
    select
        table_a.*
    from table_a
)
select
    a.*
from (
    select
        a.*
    from a
) as a";

        Assert.Equal(expect, text);


        var q2 = new SelectQuery();
        q2.With("y").As(commonY);
        var y = q2.From("y");
        q2.Select(y, "*");

        text = q2.ToQuery().CommandText;
        expect = @"with
a as (
    select
        table_a.*
    from table_a
),
y as (
    select
        a.*
    from (
        select
            a.*
        from a
    ) as a
)
select
    y.*
from y";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void Valiable()
    {
        var q1 = CreateCommonQuery("table_a", "a");
        q1.Select(":val1").As("value1").Parameter(":val1", 1);

        var q2 = new SelectQuery();
        var y = q2.From(q1).As("y");
        q2.Select(y, "*");
        q2.Select(":val2").As("value2").Parameter(":val2", 2);

        var prm = q2.ToQuery().Parameters;

        Assert.Equal(2, prm.Count);
        Assert.Equal(1, prm[":val1"]);
        Assert.Equal(2, prm[":val2"]);
    }
}
