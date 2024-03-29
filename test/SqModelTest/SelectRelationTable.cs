using SqModel;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest;

public class SelectRelationTable
{
    public SelectRelationTable(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a").As("a");
        var tb = ta.InnerJoin("table_b").As("b").On("table_a_id");
        tb.InnerJoin("table_c").As("c").On("table_b_id", "TABLE_B_ID");

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select
    *
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id
inner join table_c as c on b.table_b_id = c.TABLE_B_ID";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void CrossJoin()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a").As("a");
        ta.CrossJoin("table_b").As("b");

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select
    *
from table_a as a
cross join table_b as b";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Conditions()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a").As("a");
        var tb = ta.InnerJoin("table_b").As("b").On(x =>
           {
               x.Add().Value(10).Equal(10);
               x.Add().Column("a", "a_id").Equal("b", "b_id");
               x.AddGroup(y =>
               {
                   y.Add().Value(10).Equal(10);
                   y.Add().Or().Column("a", "a_id").Equal("b", "b_id");
               });
           });

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select
    *
from table_a as a
inner join table_b as b on 10 = 10 and a.a_id = b.b_id and (10 = 10 or a.a_id = b.b_id)";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Conditions_brief()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a").As("a");
        var tb = ta.InnerJoin("table_b").As("b").On(x =>
        {
            x.Add().Equal("rel1_id");
            x.Add().Equal("rel2_id");
        });

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select
    *
from table_a as a
inner join table_b as b on a.rel1_id = b.rel1_id and a.rel2_id = b.rel2_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Conditions_array()
    {
        var cols = new List<string>() { "rel1_id", "rel2_id" };

        var q = new SelectQuery();
        var ta = q.From("table_a").As("a");
        var tb = ta.InnerJoin("table_b").As("b").On(cols);

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select
    *
from table_a as a
inner join table_b as b on a.rel1_id = b.rel1_id and a.rel2_id = b.rel2_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Relations()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a").As("a");
        var tb = ta.InnerJoin("table_b").As("b").On("table_a_id");
        var tc = ta.LeftJoin("table_c").As("c").On("table_a_id");
        var td = tc.LeftJoin("table_d").As("d").On("table_c_id");
        var te = ta.RightJoin("table_e").As("e").On("table_a_id");
        ta.CrossJoin("table_f").As("f");

        q.SelectAll();

        var text = q.ToQuery().CommandText;
        var expect = @"select
    *
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id
left join table_c as c on a.table_a_id = c.table_a_id
left join table_d as d on c.table_c_id = d.table_c_id
right join table_e as e on a.table_a_id = e.table_a_id
cross join table_f as f";

        Assert.Equal(expect, text);

        q.GetTableClauses().ForEach(x => Output.WriteLine(x.AliasName));
    }
}
