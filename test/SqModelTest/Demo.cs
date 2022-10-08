using SqModel;
using SqModel.Analysis;
using SqModel.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class Demo
{
    [Fact]
    public void Default()
    {
        //using SqModel;
        var sq = new SelectQuery();
        var a = sq.From("table_a").As("a");
        var b = a.LeftJoin("table_b").As("b").On("id", "table_a_id");

        sq.Select(a, "id").As("a_id");
        sq.Select(b, "table_a_id").As("b_id");

        sq.Where.Add().Column(a, "id").Equal(":id").Parameter(":id", 1);
        sq.Where.Add().Column(b, "table_a_id").IsNull();
        sq.Where.Add().Column(b, "is_visible").True();

        var sql = sq.ToQuery().CommandText;

        /*
select *
from table_a as a
left join table_b as b on a.id = b.table_a_id
where
    a.id = :id
    and b.table_a_id is null
    and b.is_visible = true
        */
    }

    [Fact]
    public void Handwritten()
    {
        //using SqModel;
        //using SqModel.Analysis;
        var sq = SqlParser.Parse(@"select a.column_1 as col1, a.column_2 as col2 from table_a as a");
        var b = sq.FromClause.LeftJoin("table_b").As("b").On("id", "table_a_id");
        sq.Where.Add().Column(b, "table_a_id").IsNull();

        var sql = sq.ToQuery().CommandText;

        /*
select a.column_1 as col1, a.column_2 as col2
from table_a as a
left join table_b as b on a.id = b.table_a_id
where
    b.table_a_id is null
        */
    }

    [Fact]
    public void SelectVariable()
    {
        var sq = new SelectQuery();
        sq.Select(":val").As("value").AddParameter(":val", 1);

        var q = sq.ToQuery();
        var text = q.CommandText;

        Assert.Equal(@"select
    :val as value", text);
        Assert.Equal(1, q.Parameters[":val"]);
    }

    [Fact]
    public void TableJoin()
    {
        var sq = new SelectQuery();
        var a = sq.From("table_a").As("a");
        var b = a.InnerJoin("table_b").As("b").On("table_a_id");
        var c = b.LeftJoin("table_c").As("c").On("table_b_id", "table_b_id");
        var d = b.RightJoin("table_d").As("d").On("table_b_id", "TABLE_B_ID");
        var e = b.CrossJoin("table_e").As("e");

        sq.SelectAll();

        var q = sq.ToQuery().CommandText;
        var expect = @"select
    *
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id
left join table_c as c on b.table_b_id = c.table_b_id
right join table_d as d on b.table_b_id = d.TABLE_B_ID
cross join table_e as e";

        Assert.Equal(expect, q);
    }

    [Fact]
    public void SubQuery()
    {
        var sq = new SelectQuery();
        sq.From(x =>
        {
            x.From("table_a").As("a");
            x.SelectAll();
        }).As("aa");
        sq.SelectAll();

        var q = sq.ToQuery().CommandText;
        var expect = @"select
    *
from (
    select
        *
    from table_a as a
) as aa";

        Assert.Equal(expect, q);
    }

    [Fact]
    public void Condition()
    {
        var sq = new SelectQuery();
        var a = sq.From("table_a").As("a");
        sq.SelectAll();
        sq.Where.Add().Column(a, "id").Equal(":id1").Parameter(":id", 1);
        sq.Where.Add().Column("a", "id").Equal(":id2").Parameter(":id", 2);
        sq.Where.Add().Column(a, "id").Equal(10);
        sq.Where.Add().Column(a, "id").IsNull();
        sq.Where.Add().Column(a, "id").IsNotNull();
        sq.Where.Add().Column(a, "id").True();
        sq.Where.Add().Column(a, "id").False();
        sq.Where.Add().Column(a, "id").Right = new CommandValue() { Conjunction = ">=", CommandText = "10" };

        var q = sq.ToQuery();
        var expect = @"select
    *
from table_a as a
where
    a.id = :id1
    and a.id = :id2
    and a.id = 10
    and a.id is null
    and a.id is not null
    and a.id = true
    and a.id = false
    and a.id >= 10";

        Assert.Equal(expect, q.CommandText);
    }

    [Fact]
    public void ConditionGroup()
    {
        var sq = new SelectQuery();
        var a = sq.From("table_a").As("a");
        sq.SelectAll();
        sq.Where.AddGroup(x =>
        {
            x.Add().Or().Column(a, "id").Equal(1);
            x.Add().Or().Column(a, "id").Equal(2);
        });
        sq.Where.Add().Column(a, "id").Equal(3);

        var q = sq.ToQuery();
        var expect = @"select
    *
from table_a as a
where
    (a.id = 1 or a.id = 2)
    and a.id = 3";

        Assert.Equal(expect, q.CommandText);
    }

    [Fact]
    public void ExistsNotExists()
    {
        var sq = new SelectQuery();
        var a = sq.From("table_a").As("a");
        sq.SelectAll();
        sq.Where.Add().Exists(x =>
        {
            var b = x.From("table_b").As("b");
            x.SelectAll();
            x.Where.Add().Column(b, "id").Equal(a, "id");
        });
        sq.Where.Add().Not().Exists(x =>
        {
            var c = x.From("table_c").As("c");
            x.SelectAll();
            x.Where.Add().Column(c, "id").Equal(a, "id");
        });

        var q = sq.ToQuery();
        var expect = @"select
    *
from table_a as a
where
    exists (select * from table_b as b where b.id = a.id)
    and not exists (select * from table_c as c where c.id = a.id)";

        Assert.Equal(expect, q.CommandText);
    }

    [Fact]
    public void CommonTable()
    {
        var sq = new SelectQuery();
        var cta = sq.With.Add(x =>
        {
            x.From("table_a");
            x.SelectAll();
        }).As("a");

        var ctb = sq.With.Add(x =>
        {
            x.From("table_b");
            x.SelectAll();
        }).As("b");

        var a = sq.From(cta);
        a.InnerJoin(ctb).On("id");
        sq.SelectAll();

        var q = sq.ToQuery().CommandText;
        var expect = @"with
a as (
    select
        *
    from table_a
),
b as (
    select
        *
    from table_b
)
select
    *
from a
inner join b on a.id = b.id";

        Assert.Equal(expect, q);
    }

    [Fact]
    public void CreateTable()
    {
        var sq = new SelectQuery();
        var a = sq.From("table_a");
        sq.Select(a, "*");

        var tq = new CreateTableQuery() { SelectQuery = sq, TableName = "tmp" };

        var q = tq.ToQuery().CommandText;
        var expect = @"create table tmp
as
select
    table_a.*
from table_a";

        Assert.Equal(expect, q);
    }

    [Fact]
    public void CreateView()
    {
        var sq = new SelectQuery();
        var a = sq.From("table_a");
        sq.Select(a, "*");

        var tq = new CreateViewQuery() { SelectQuery = sq, ViewName = "tmp" };

        var q = tq.ToQuery().CommandText;
        var expect = @"create view tmp
as
select
    table_a.*
from table_a";

        Assert.Equal(expect, q);
    }

    [Fact]
    public void InsertQuery()
    {
        var sq = new SelectQuery();
        var a = sq.From("table_a").As("a");
        sq.Select(a, "id").As("index_value");

        var tq = new InsertQuery() { SelectQuery = sq, TableName = "table_b" };

        var q = tq.ToQuery().CommandText;
        var expect = @"insert into table_b(index_value)
select
    a.id as index_value
from table_a as a";

        Assert.Equal(expect, q);
    }

    [Fact]
    public void ParseHandwrittenSql()
    {
        var sq = SqlParser.Parse(@"select a.column_1 as col1, b.column_2 as col2 from table_a as a inner join table_b as b on a.id = b.id");
        var q = sq.ToQuery().CommandText;
        var expect = @"select
    a.column_1 as col1
    , b.column_2 as col2
from table_a as a
inner join table_b as b on a.id = b.id";
        Assert.Equal(expect, q);
    }
}
