# SqModel
A lightweight library that allows you to easily build Select queries.
You can also parse handwritten Sql.

## Demo
You can compose your select query like this:
```cs
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
```

```sql
select a.id as a_id, b.table_a_id as b_id
from table_a as a
left join table_b as b on a.id = b.table_a_id
where
    a.id = :id
    and b.table_a_id is null
    and b.is_visible = true
```

It is also possible to parse handwritten Select queries into the SqModel class.

```cs
//using SqModel;
//using SqModel.Analysis;
var sq = SqlParser.Parse(@"select a.column_1 as col1, a.column_2 as col2 from table_a as a");
var b = sq.FromClause.LeftJoin("table_b").As("b").On("id", "table_a_id");
sq.Where.Add().Column(b, "table_a_id").IsNull();

var sql = sq.ToQuery().CommandText;
```

```sql
select a.column_1 as col1, a.column_2 as col2
from table_a as a
left join table_b as b on a.id = b.table_a_id
where
    b.table_a_id is null
```

## Feature
- Method name (From, Select, Join, Where etc) in SQL syntax.
- Supports table aliases and column aliases.
- Supports parameter queries.
- Supports subqueries.
- Corresponds to the DISTINCT keyword.
- Supports CTE (Common Table Expression).
- Supports table creation queries.
- Supports view creation queries.
- Supports insert queries.
- Modest SQL formatting.
- Handwritten Sql parsing.

## Constraints
- There is no SQL syntax check function.
- Does not support SQL execution. Use the library of extension methods.

https://github.com/mk3008/SqModel.Dapper

https://www.nuget.org/packages/SqModel.Dapper

## Execution environment
.NET6

https://www.nuget.org/packages/SqModel/

## Sample
### Parameter query using variables
```cs
[Fact]
public void SelectVariable()
{
    var sq = new SelectQuery();
    sq.Select(":val").As("value").Parameter(":val", 1);

    var q = sq.ToQuery();
    var text = q.CommandText;

    Assert.Equal("select :val as value", text);
    Assert.Equal(1, q.Parameters[":val"]);
}
```

### Table join(version 0.4 or later)
```cs
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
    var expect = @"select *
from table_a as a
inner join table_b as b on a.table_a_id = b.table_a_id
left join table_c as c on b.table_b_id = c.table_b_id
right join table_d as d on b.table_b_id = d.TABLE_B_ID
cross join table_e as e";

    Assert.Equal(expect, q);
}
```

### Subquery
```cs
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
    var expect = @"select *
from (
select *
from table_a as a
) as aa";

    Assert.Equal(expect, q);
}
```

### Extraction condition(version 0.4 or later)
```cs
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
    var expect = @"select *
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
```

### Extraction condition(group)(version 0.4 or later)
```cs
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
    var expect = @"select *
from table_a as a
where
(a.id = 1 or a.id = 2)
and a.id = 3";

    Assert.Equal(expect, q.CommandText);
}
```

### Use only extraction conditions(version 0.4 or later)
```cs
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
    var expect = @"select *
from table_a as a
where
exists (select * from table_b as b where b.id = a.id)
and not exists (select * from table_c as c where c.id = a.id)";

    Assert.Equal(expect, q.CommandText);
}
```

### CTE
```cs
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
select *
from table_a
),
b as (
select *
from table_b
)
select *
from a
inner join b on a.id = b.id";

    Assert.Equal(expect, q);
}
```

### Table creation query
```cs
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
select table_a.*
from table_a";

    Assert.Equal(expect, q);
}
```

### View creation query
```cs
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
select table_a.*
from table_a";

    Assert.Equal(expect, q);
}
```

### Insert query
```cs
[Fact]
public void InsertQuery()
{
    var sq = new SelectQuery();
    var a = sq.From("table_a").As("a");
    sq.Select(a, "id").As("index_value");

    var tq = new InsertQuery() { SelectQuery = sq, TableName = "table_b" };

    var q = tq.ToQuery().CommandText;
    var expect = @"insert into table_b(index_value)
select a.id as index_value
from table_a as a";

    Assert.Equal(expect, q);
}
````

### Parse(version 0.4 or later)
You can parse handwritten SQL and use it as SqModel.
Table joins and inline queries can be parsed, but there are patterns that cannot be parsed (ex.group by, order by).

```cs
[Fact]
public void ParseHandwrittenSql()
{
    var sq = SqlParser.Parse(@"select a.column_1 as col1, b.column_2 as col2 from table_a as a inner join table_b as b on a.id = b.id");
    var q = sq.ToQuery().CommandText;
    var expect = @"select a.column_1 as col1, b.column_2 as col2
from table_a as a
inner join table_b as b on a.id = b.id";
    Assert.Equal(expect, q);
}
```

### CaseExpression(version 0.4 or later)
```cs
[Fact]
public void DefaultCaseWhen()
{
    var sq = new SelectQuery();
    var a = sq.From("table_a").As("a");

    sq.Select.Add().CaseWhen(x =>
    {
        x.Add().When(w => w.Value("a").Equal(1)).Then(10);
        x.Add().When(w => w.Column("a", "id").Equal(2)).Then(20);
        x.Add().When(w => w.Column(a, "id").Equal(3)).Then(30);
        x.Add().WhenGroup(g =>
        {
            g.Add().Column("a", "id").Equal(1);
            g.Add().Or().Column("b", "id").Equal(2);
        }).Then(40);
    }).As("case_1");

    var q = sq.ToQuery().CommandText;
    var expect = @"select case when a = 1 then 10 when a.id = 2 then 20 when a.id = 3 then 30 when (a.id = 1 or b.id = 2) then 40 end as case_1
from table_a as a";

    Assert.Equal(expect, q);
}

[Fact]
public void DefaultCase()
{
    var sq = new SelectQuery();
    var a = sq.From("table_a").As("a");

    sq.Select.Add().Case("1", x =>
    {
        x.Add().When("a").Then(10);
        x.Add().When("a", "id").Then(20);
        x.Add().When(a, "id").Then(30);
        x.Add().When(1).Then(30);
        x.Add().When(1).ThenNull();
        x.Add().Else(100);
    }).As("case_2");

    var q = sq.ToQuery().CommandText;
    var expect = @"select case 1 when a then 10 when a.id then 20 when a.id then 30 when 1 then 30 when 1 then null else 100 end as case_2
from table_a as a";

    Assert.Equal(expect, q);
}
```
