using SqModel;
using System.Collections.Generic;
using Xunit;

namespace SqModelTest;

public class SelectRelationTable
{
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        var table_b = table_a.InnerJoin("table_b", new() { "table_a_id" });

        q.Select(table_a, "value_a");
        q.Select(table_b, "value_b");

        var text = q.ToQuery().CommandText;
        var expect = @"select table_a.value_a, table_b.value_b
from table_a
inner join table_b on table_a.table_a_id = table_b.table_a_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void InnerJoin()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        var table_b = table_a.InnerJoin("table_b", "b", new() { "table_a_id" });

        q.Select(table_a, "value_a");
        q.Select(table_b, "value_b");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.value_a, b.value_b
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void JoinConditions()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        var table_b = table_a.InnerJoin("table_b", "b", new() { "table_a_id", "table_a_sub_id" });

        q.Select(table_a, "value_a");
        q.Select(table_b, "value_b");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.value_a, b.value_b
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id and a.table_a_sub_id = b.table_a_sub_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void JoinCondition()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");

        var dic = new Dictionary<string, string>();
        dic.Add("table_a_id", "table_b_id");
        var table_b = table_a.Join("table_b","b", TableRelationClause.RelationTypes.Inner, dic);

        q.Select(table_a, "value_a");
        q.Select(table_b, "value_b");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.value_a, b.value_b
from table_a as a
inner join table_b as b on a.table_a_id = b.table_b_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void LeftJoin()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        var table_b = table_a.LeftJoin("table_b", "b", new() { "table_a_id" });

        q.Select(table_a, "value_a");
        q.Select(table_b, "value_b");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.value_a, b.value_b
from table_a as a
left  join table_b as b on a.table_a_id = b.table_a_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void RightJoin()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        var table_b = table_a.RightJoin("table_b", "b", new() { "table_a_id" });

        q.Select(table_a, "value_a");
        q.Select(table_b, "value_b");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.value_a, b.value_b
from table_a as a
right join table_b as b on a.table_a_id = b.table_a_id";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void CrossJoin()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        var table_b = table_a.CrossJoin("table_b", "b");

        q.Select(table_a, "value_a");
        q.Select(table_b, "value_b");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.value_a, b.value_b
from table_a as a
cross join table_b as b";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Relations()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        var table_b = table_a.InnerJoin("table_b", "b", new() { "table_a_id" });
        var table_c = table_a.LeftJoin("table_c", "c", new() { "table_a_id" });
        var table_d = table_c.LeftJoin("table_d", "d", new() { "table_c_id" });
        var table_e = table_d.CrossJoin("table_e", "e");

        q.Select(table_a, "value_a");
        q.Select(table_b, "value_b");
        q.Select(table_c, "value_c");
        q.Select(table_d, "value_d");
        q.Select(table_e, "value_e");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.value_a, b.value_b, c.value_c, d.value_d, e.value_e
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id
left  join table_c as c on a.table_a_id = c.table_a_id
left  join table_d as d on c.table_c_id = d.table_c_id
cross join table_e as e";

        Assert.Equal(expect, text);
    }
}
