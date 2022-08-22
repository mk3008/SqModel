using SqModel;
using SqModel.Building;
using System.Collections.Generic;
using Xunit;

namespace SqModelTest;

public class SelectRelationTable
{
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a", "a");
        var tb = ta.InnerJoin("table_b", "b").On("table_a_id");
        tb.InnerJoin("table_c", "c").On("table_b_id", "TABLE_B_ID");

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select *
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id
inner join table_c as c on b.table_b_id = c.TABLE_B_ID";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void CrossJoin()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a", "a");
        ta.CrossJoin("table_b", "b");

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select *
from table_a as a
cross join table_b as b";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Conditions()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a", "a");
        ta.InnerJoin("table_b", "b").On(x =>
        {
            x.Add().Equal("id");
            x.Add().Equal("a_id", "b_id");
            x.Add().Group(y =>
            {
                y.Add().Or().Equal("id3");
                y.Add().Or().Equal("id4");
            });
            x.Add().Value("x", "id5").Equal("y", "id6");
        });

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select *
from table_a as a
inner join table_b as b on a.id = b.id and a.a_id = b.b_id and (a.id3 = b.id3 or a.id4 = b.id4) and x.id5 = y.id6";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Relations()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a", "a");
        var tb = ta.InnerJoin("table_b", "b").On("table_a_id");
        var tc = ta.LeftJoin("table_c", "c").On("table_a_id");
        var td = tc.LeftJoin("table_d", "d").On("table_c_id");
        var te = ta.RightJoin("table_e", "e").On("table_a_id");
        ta.CrossJoin("table_f", "f");

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select *
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id
left join table_c as c on a.table_a_id = c.table_a_id
left join table_d as d on c.table_c_id = d.table_c_id
right join table_e as e on a.table_a_id = e.table_a_id
cross join table_f as f";

        Assert.Equal(expect, text);
    }
}
