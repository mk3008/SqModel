using SqModel;
using Xunit;

namespace SqModelTest;

public class WhereQuery
{
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where.Add().Value(table_a, "id").Equal(":id").AddParameter(":id", 1);

        var acutal = q.ToQuery();
        var expect = @"select table_a.*
from table_a
where
    table_a.id = :id";

        Assert.Equal(expect, acutal.CommandText);
        Assert.Single(acutal.Parameters);
        Assert.Equal(1, acutal.Parameters[":id"]);
    }

    [Fact]
    public void CommandText()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where.Add().Value("table_a.id").NotEqual(":id").AddParameter(":id", 1);

        var acutal = q.ToQuery();
        var expect = @"select table_a.*
from table_a
where
    table_a.id <> :id";

        Assert.Equal(expect, acutal.CommandText);
        Assert.Single(acutal.Parameters);
        Assert.Equal(1, acutal.Parameters[":id"]);
    }

    [Fact]
    public void OperatorAnd()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where.Add().Value("table_a.id").Equal(":id").AddParameter(":id", 1);
        q.Where.Add().Value("table_a.sub_id").Equal(":sub_id").AddParameter(":sub_id", 2);

        var acutal = q.ToQuery();
        var expect = @"select table_a.*
from table_a
where
    table_a.id = :id
    and table_a.sub_id = :sub_id";

        Assert.Equal(expect, acutal.CommandText);
        Assert.Equal(2, acutal.Parameters.Count);
        Assert.Equal(1, acutal.Parameters[":id"]);
        Assert.Equal(2, acutal.Parameters[":sub_id"]);
    }

    [Fact]
    public void OperatorOr()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where.Add().Group(x =>
        {
            x.Add().Or().Value("table_a.id").Equal(":id1").AddParameter(":id1", 1);
            x.Add().Or().Value("table_a.id").Equal(":id2").AddParameter(":id2", 2);
        });

        var acutal = q.ToQuery();
        var expect = @"select table_a.*
from table_a
where
    (table_a.id = :id1 or table_a.id = :id2)";

        Assert.Equal(expect, acutal.CommandText);
        Assert.Equal(2, acutal.Parameters.Count);
        Assert.Equal(1, acutal.Parameters[":id1"]);
        Assert.Equal(2, acutal.Parameters[":id2"]);
    }

    [Fact]
    public void OperatorComposite()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");

        q.Where.Add().Group(x =>
        {
            x.Add().Or().Value("table_a.id").Equal(":id1").AddParameter(":id1", 1);
            x.Add().Or().Value("table_a.id").Equal(":id2").AddParameter(":id2", 2);
        });
        q.Where.Add().Value("table_a.sub_id").Equal(":sub_id").AddParameter(":sub_id", 2);

        var acutal = q.ToQuery();
        var expect = @"select table_a.*
from table_a
where
    (table_a.id = :id1 or table_a.id = :id2)
    and table_a.sub_id = :sub_id";

        Assert.Equal(expect, acutal.CommandText);
        Assert.Equal(3, acutal.Parameters.Count);
        Assert.Equal(1, acutal.Parameters[":id1"]);
        Assert.Equal(2, acutal.Parameters[":id2"]);
        Assert.Equal(2, acutal.Parameters[":sub_id"]);
    }

    [Fact]
    public void WhereOnly()
    {
        var q = new SelectQuery();
        q.Where.Add().Group(x =>
        {
            x.Add().Or().Value("table_a.id").Equal(":id1").AddParameter(":id1", 1);
            x.Add().Or().Value("table_a.id").Equal(":id2").AddParameter(":id2", 2);
        });

        q.Where.Add().Value("table_a", "id").Equal("table_b", "id");
        q.Where.Add().Value("table_a", "id").Equal(":sub_id").AddParameter(":sub_id", 2);
        q.Where.Add().Value("table_a", "id").IsNull();
        q.Where.Add().Value("table_a", "id").IsNotNull();

        q.Where.Add().Group(x =>
        {
            x.Add().Or().Value("table_a.id").Equal(":id1").AddParameter(":id1", 1);
            x.Add().Or().Value("table_a.id").Equal(":id2").AddParameter(":id2", 2);
        });

        q.Where.Add().Exists(() =>
        {
            var x = new SelectQuery();
            x.From("table_x", "x");
            x.SelectAll();
            x.Where.Add().Value("x.id").Equal("table_a.id");
            return x;
        });

        q.Where.Add().Not().Exists(() =>
        {
            var x = new SelectQuery();
            x.From("table_x", "x");
            x.SelectAll();
            x.Where.Add().Value("x.id").Equal("table_a.id");
            return x;
        });

        var acutal = q.WhereClause.ToQuery();
        var expect = @"where
    (table_a.id = :id1 or table_a.id = :id2)
    and table_a.id = table_b.id
    and table_a.id = :sub_id
    and table_a.id is null
    and table_a.id is not null
    and (table_a.id = :id1 or table_a.id = :id2)
    and exists (
        select *
        from table_x as x
        where
            x.id = table_a.id
    )
    and not exists (
        select *
        from table_x as x
        where
            x.id = table_a.id
    )";

        Assert.Equal(expect, acutal.CommandText);
        Assert.Equal(3, acutal.Parameters.Count);
        Assert.Equal(1, acutal.Parameters[":id1"]);
        Assert.Equal(2, acutal.Parameters[":id2"]);
        Assert.Equal(2, acutal.Parameters[":sub_id"]);
    }
}
