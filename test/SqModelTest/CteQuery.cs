using SqModel;
using SqModel.Building;
using Xunit;

namespace SqModelTest;

public class CteQuery
{
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var ta = q.With(x =>
        {
            x.From("table_a");
            x.SelectAll();
        }, "a");

        q.From(ta);
        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"with
a as (
    select *
    from table_a
)
select *
from a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Many()
    {
        var q = new SelectQuery();
        var cta = q.With(x =>
        {
            x.From("table_a");
            x.SelectAll();
        }, "a");
        var ctb = q.With(x =>
        {
            x.From("table_b");
            x.SelectAll();
        }, "b");
        var ctc = q.With(x =>
        {
            x.From("table_c");
            x.SelectAll();
        }, "c");

        var ta = q.From(cta);
        var tb = ta.InnerJoin(ctb).On("table_a_id");
        tb.InnerJoin(ctc).On("table_b_id");

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"with
a as (
    select *
    from table_a
),
b as (
    select *
    from table_b
),
c as (
    select *
    from table_c
)
select *
from a
inner join b on a.table_a_id = b.table_a_id
inner join c on b.table_b_id = c.table_b_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void PushToCommonTable()
    {
        var q = new SelectQuery();
        q.From("table_a");
        q.SelectAll();

        q = q.PushToCommonTable("a");

        q.From("a");
        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"with
a as (
    select *
    from table_a
)
select *
from a";

        Assert.Equal(expect, text);
    }
}
