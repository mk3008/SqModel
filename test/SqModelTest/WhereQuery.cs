using SqModel;
using SqModel.Expression;
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
        q.Where.Add().Column(table_a, "id").Equal(":id").Parameter(":id", 1);

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
        q.Where.Add().Value("table_a.id").NotEqual(":id").Parameter(":id", 1);

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
        q.Where.Add().Value("table_a.id").Equal(":id").Parameter(":id", 1);
        q.Where.Add().Value("table_a.sub_id").Equal(":sub_id").Parameter(":sub_id", 2);

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
        q.Where.AddGroup(x =>
        {
            x.Add().Or().Value("table_a.id").Equal(":id1").Parameter(":id1", 1);
            x.Add().Or().Value("table_a.id").Equal(":id2").Parameter(":id2", 2);
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

        q.Where.AddGroup(x =>
        {
            x.Add().Or().Value("table_a.id").Equal(":id1").Parameter(":id1", 1);
            x.Add().Or().Value("table_a.id").Equal(":id2").Parameter(":id2", 2);
        });
        q.Where.Add().Value("table_a.sub_id").Equal(":sub_id").Parameter(":sub_id", 2);

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
        q.Where.AddGroup(x =>
        {
            x.Add().Or().Value("table_a.id").Equal(":id1").Parameter(":id1", 1);
            x.Add().Or().Value("table_a.id").Equal(":id2").Parameter(":id2", 2);
        });

        q.Where.Add().Column("table_a", "id").Equal("table_b", "id");
        q.Where.Add().Column("table_a", "id").Equal(":sub_id").Parameter(":sub_id", 2);
        q.Where.Add().Column("table_a", "id").IsNull();
        q.Where.Add().Column("table_a", "id").IsNotNull();

        q.Where.AddGroup(x =>
        {
            x.Add().Or().Value("table_a.id").Equal(":id1").Parameter(":id1", 1);
            x.Add().Or().Value("table_a.id").Equal(":id2").Parameter(":id2", 2);
        });

        q.Where.Add().Exists(x =>
        {
            x.From("table_x").As("x");
            x.SelectAll();
            x.Where.Add().Equal("x", "table_a", "id");
        });

        q.Where.Add().Not().Exists(x =>
        {
            x.From("table_x").As("x");
            x.SelectAll();
            x.Where.Add().Column("x", "id").Equal("table_a", "id");
        });

        q.Where.Add().Case("table_a.id", x =>
        {
            x.Add().When("1").Then("10");
            x.Add().When("2").Then("20");
            x.Add().When("3").Then("30");
        }).Equal("1");

        q.Where.Add().CaseWhen(x =>
        {
            x.Add().When(w => w.Column("table_a", "id").Equal("1")).Then("10");
            x.Add().When(w => w.Column("table_b", "id").Equal("2")).Then("20");
            x.Add().When(w => w.Column("table_c", "id").Equal("3")).Then("30");
        }).Equal("1");

        var acutal = q.WhereClause.ToQuery();
        var expect = @"where
    (table_a.id = :id1 or table_a.id = :id2)
    and table_a.id = table_b.id
    and table_a.id = :sub_id
    and table_a.id is null
    and table_a.id is not null
    and (table_a.id = :id1 or table_a.id = :id2)
    and exists (select * from table_x as x where x.id = table_a.id)
    and not exists (select * from table_x as x where x.id = table_a.id)
    and case table_a.id when 1 then 10 when 2 then 20 when 3 then 30 end = 1
    and case when table_a.id = 1 then 10 when table_b.id = 2 then 20 when table_c.id = 3 then 30 end = 1";

        var text = acutal.CommandText;
        Assert.Equal(expect, text);
        Assert.Equal(3, acutal.Parameters.Count);
        Assert.Equal(1, acutal.Parameters[":id1"]);
        Assert.Equal(2, acutal.Parameters[":id2"]);
        Assert.Equal(2, acutal.Parameters[":sub_id"]);
    }
}
