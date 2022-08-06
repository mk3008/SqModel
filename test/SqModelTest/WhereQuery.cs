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
        q.WhereAnd(table_a, "id", ":id", 1);

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
        q.WhereAnd("table_a.id", "<>", ":id").Destination.AddParameter(":id", 1);

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
        q.WhereAnd("table_a.id", "=", ":id").Destination.AddParameter(":id", 1);
        q.WhereAnd("table_a.sub_id", "=", ":sub_id").Destination.AddParameter(":sub_id", 2);

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
        q.WhereAnd(g =>
        {
            g.WhereOr("table_a.id", "=", ":id1").Destination.AddParameter(":id1", 1);
            g.WhereOr("table_a.id", "=", ":id2").Destination.AddParameter(":id2", 2);
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

        q.WhereAnd(g =>
        {
            g.WhereOr("table_a.id", "=", ":id1").Destination.AddParameter(":id1", 1);
            g.WhereOr("table_a.id", "=", ":id2").Destination.AddParameter(":id2", 2);
        });
        q.WhereAnd("table_a.sub_id", "=", ":sub_id").Destination.AddParameter(":sub_id", 2);

        var acutal = q.ToQuery();
        var expect = @"select table_a.*
from table_a
where
    table_a.sub_id = :sub_id
    and (table_a.id = :id1 or table_a.id = :id2)";

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
        q.WhereAnd( g =>
        {
            g.WhereOr("table_a.id", "=", ":id1").Destination.AddParameter(":id1", 1);
            g.WhereOr("table_a.id", "=", ":id2").Destination.AddParameter(":id2", 2);
        });
        q.WhereAnd("table_a.sub_id", "=", ":sub_id").Destination.AddParameter(":sub_id", 2);

        var acutal = q.WhereClause.ToQuery();
        var expect = @"where
    table_a.sub_id = :sub_id
    and (table_a.id = :id1 or table_a.id = :id2)";

        Assert.Equal(expect, acutal.CommandText);
        Assert.Equal(3, acutal.Parameters.Count);
        Assert.Equal(1, acutal.Parameters[":id1"]);
        Assert.Equal(2, acutal.Parameters[":id2"]);
        Assert.Equal(2, acutal.Parameters[":sub_id"]);
    }
}
