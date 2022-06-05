using SqModel;
using Xunit;

namespace SqModelTest;

public class CteQuery
{
    [Fact]
    public void Default()
    {
        var sub = new SelectQuery();
        var table_a = sub.From("table_a");
        sub.Select(table_a, "*");

        var q = new SelectQuery();
        q.With.Add(sub, "a");
        var a = q.From("a");
        q.Select(a, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"with
a as (
    select table_a.*
    from table_a
)
select a.*
from a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Many()
    {
        var sub1 = new SelectQuery();
        var table_a = sub1.From("table_a");
        sub1.Select(table_a, "*");

        var sub2 = new SelectQuery();
        var table_b = sub2.From("table_b");
        sub2.Select(table_b, "*");

        var q = new SelectQuery();
        q.With.Add(sub1, "a");
        q.With.Add(sub2, "b");

        var a = q.From("a");
        var b = a.InnerJoin("b", "b", new() { "table_a_id" });
        q.Select(a, "*");
        q.Select(b, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"with
a as (
    select table_a.*
    from table_a
),
b as (
    select table_b.*
    from table_b
)
select a.*, b.*
from a
inner join b on a.table_a_id = b.table_a_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Valiable()
    {
        var sub1 = new SelectQuery();
        var table_a = sub1.From("table_a");
        sub1.Select(table_a, "*");
        sub1.Select(":val1", "val1").AddParameter(":val1", 1);

        var sub2 = new SelectQuery();
        var table_b = sub2.From("table_b");
        sub2.Select(table_b, "*");
        sub2.Select(":val2", "val2").AddParameter(":val2", 2);

        var q = new SelectQuery();
        q.With.Add(sub1, "a");
        q.With.Add(sub2, "b");

        var a = q.From("a");
        var b = a.InnerJoin("b", "b", new() { "table_a_id" });
        q.Select(a, "*");
        q.Select(b, "*");

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"with
a as (
    select table_a.*, :val1 as val1
    from table_a
),
b as (
    select table_b.*, :val2 as val2
    from table_b
)
select a.*, b.*
from a
inner join b on a.table_a_id = b.table_a_id";

        Assert.Equal(expect, text);
        Assert.Equal(1, actual.Parameters[":val1"]);
        Assert.Equal(2, actual.Parameters[":val2"]);
    }
}
