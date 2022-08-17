using SqModel;
using Xunit;

namespace SqModelTest;

public class SelectSubquery
{
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        q.From(x =>
        {
            x.From("table_a", "a");
            x.SelectAll();
        }, "b");
        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select *
from (
    select *
    from table_a as a
) as b";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void SubQueries()
    {
        var q = new SelectQuery();
        var tb1 = q.From(x =>
        {
            x.From("table_a1", "a1");
            x.SelectAll();
        }, "b1");
        tb1.InnerJoin(x =>
        {
            x.From("table_a2", "a2");
            x.SelectAll();
        }, "b2").On("id");

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select *
from (
    select *
    from table_a1 as a1
) as b1
inner join (
    select *
    from table_a2 as a2
) as b2 on b1.id = b2.id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Nest()
    {
        var q = new SelectQuery();
        q.From(x =>
        {
            x.From(y =>
            {
                y.From(z =>
                {
                    z.From("table_z", "z");
                    z.SelectAll();
                }, "y");
                y.SelectAll();
            }, "x");
            x.SelectAll();
        }, "a");

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select *
from (
    select *
    from (
        select *
        from (
            select *
            from table_z as z
        ) as y
    ) as x
) as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Composition()
    {
        var q1 = new SelectQuery();
        q1.From("table_a", "a");
        q1.SelectAll();

        var q2 = new SelectQuery();
        q2.From(q1, "b");
        q2.SelectAll();

        var q3 = new SelectQuery();
        q3.From(q2, "c");
        q3.SelectAll();

        var text = q3.ToQuery().CommandText;
        var expect = @"select *
from (
    select *
    from (
        select *
        from table_a as a
    ) as b
) as c";

        Assert.Equal(expect, text);
    }
}
