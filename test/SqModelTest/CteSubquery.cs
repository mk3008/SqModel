using SqModel;
using Xunit;

namespace SqModelTest;

public class CteSubquery
{
    private SelectQuery GetQuery()
    {
        var commonQuery = new SelectQuery();
        var table_a = commonQuery.From("table_a");
        commonQuery.Select(table_a, "*");
        commonQuery.Select(":val", "value").Parameters.Add(":val", 1);

        var q = new SelectQuery();
        q.With.Add(commonQuery, "a");
        var a = q.From("a");
        q.Select(a, "*");

        return q;
    }

    [Fact]
    public void Default()
    {
        var q1 = GetQuery();

        var text = q1.ToQuery().CommandText;
        var expect = @"with
a as (
    select table_a.*, :val as value
    from table_a
)
select a.*
from a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Nest()
    {
        var q1 = GetQuery();

        var q2 = new SelectQuery();
        var y = q2.From(q1, "y");
        q2.Select(y, "*");

        var text = q2.ToQuery().CommandText;
        var expect = @"with
a as (
    select table_a.*, :val as value
    from table_a
)
select y.*
from (
    select a.*
    from a
) as y";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Valiable()
    {
        var q1 = GetQuery();

        var q2 = new SelectQuery();
        var y = q2.From(q1, "y");
        q2.Select(y, "*");

        var prm = q2.ToQuery().Parameters;
 
        Assert.Single(prm);
        Assert.Equal(1, prm[":val"]);
    }
}
