using SqModel;
using Xunit;

namespace SqModelTest;

public class SelectSubquery
{
    [Fact]
    public void Default()
    {
        var sub = new SelectQuery();
        var table_a = sub.From("table_a");
        sub.Select(table_a, "*");

        var q = new SelectQuery();
        var a = q.From(sub, "a");
        q.Select(a, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.*
from table_a as a";

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
        var a = q.From(sub1, "a");
        var b = a.InnerJoin(sub2, "b", new() { "table_a_id" });
        q.Select(a, "*");
        q.Select(b, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.*, b.*
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Valiable()
    {
        var q = new SelectQuery();
        var a = q.From(sq =>
        {
            var t = sq.From("table_a");
            sq.Select(t, "*");
            sq.Select(":val1", "val1").AddParameter(":val1", 1);
        }
        , "a");

        var b = a.InnerJoin(sq =>
        {
            var t = sq.From("table_b");
            sq.Select(t, "*");
            sq.Select(":val2", "val2").AddParameter(":val2", 2);
        }, "b", new() { "table_a_id" });

        q.Select(a, "*");
        q.Select(b, "*");

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select a.*, b.*
from (
    select table_a.*, :val1 as val1
    from table_a
) as a
inner join (
    select table_b.*, :val2 as val2
    from table_b
) as b on a.table_a_id = b.table_a_id";

        Assert.Equal(expect, text);
        Assert.Equal(1, actual.Parameters[":val1"]);
        Assert.Equal(2, actual.Parameters[":val2"]);
    }
}
