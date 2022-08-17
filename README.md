# SqModel
A lightweight library that allows you to easily build Select queries.
You can also parse handwritten Sql.

## Demo
You can compose your select query like this:
```cs
var sq = new SelectQuery();
var ta = sq.From("table_a", "a");
var tb = ta.LeftJoin("table_b", "b").On("id", "table_a_id");

sq.Select(ta, "id");
sq.Select(tb, "table_a_id", "a_id");

sq.Where().Value(ta, "id").Equal(":id").AddParameter(":id", 1);
sq.Where().Value(tb, "table_a_id").IsNull();

var q = sq.ToQuery();

/*
select a.id, b.table_a_id as a_id
from table_a as a
left join table_b as b on a.id = b.table_a_id
where
a.id = :id --1
and b.table_a_id is null
*/
Console.WriteLine(q.CommandText);
```

It is also possible to parse handwritten Select queries into the SqModel class.

```cs
using var p = new Parser(@"select a.column_1 as col1, a.column_2 as col2 from table_a as a");
var q = p.ParseSelectQuery();
var text = q.ToQuery().CommandText;
var expect = @"select a.column_1 as col1, a.column_2 as col2
from table_a as a";
Assert.Equal(expect, text);

Assert.Equal("a", q.SelectClause.ColumnClauses[0].TableName);
Assert.Equal("column_1", q.SelectClause.ColumnClauses[0].Value);
Assert.Equal("col1", q.SelectClause.ColumnClauses[0].AliasName);

Assert.Equal("table_a", q.FromClause.TableName);
Assert.Equal("a", q.FromClause.AliasName);
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
### Single table selection
```cs
    [Fact]
    public void TableNameAlias()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(table_a, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.*
from table_a as a";

        Assert.Equal(expect, text);
    }
```

### Get columns
```cs
    [Fact]
    public void SelectColumnWithAlias()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(table_a, "column_x", "x");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.column_x as x
from table_a as a";

        Assert.Equal(expect, text);
    }
```

### Parameter query using variables
```cs
    [Fact]
    public void SelectVariable()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(":val", "value").AddParameter(":val", 1);

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select :val as value
from table_a as a";

        Assert.Equal(expect, text);
        Assert.Equal(1, actual.Parameters[":val"]);
    }
```

### Table join(version 0.4 or later)
```cs
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
```

### Subquery
```cs
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
```

### Extraction condition(version 0.4 or later)
```cs
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where().Value(table_a, "id").Equal(":id").AddParameter(":id", 1);

        var acutal = q.ToQuery();
        var expect = @"select table_a.*
from table_a
where
    table_a.id = :id";

        Assert.Equal(expect, acutal.CommandText);
        Assert.Single(acutal.Parameters);
        Assert.Equal(1, acutal.Parameters[":id"]);
    }
```

### Extraction condition(group)(version 0.4 or later)
```cs
    [Fact]
    public void OperatorOr()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where().Group(x =>
        {
            x.Where().Or.Value("table_a.id").Equal(":id1").AddParameter(":id1", 1);
            x.Where().Or.Value("table_a.id").Equal(":id2").AddParameter(":id2", 2);
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
```

### Use only extraction conditions(version 0.4 or later)
```cs
    [Fact]
    public void WhereOnly()
    {
        var q = new SelectQuery();
        q.Where().Group(x =>
        {
            x.Where().Or.Value("table_a.id").Equal(":id1").AddParameter(":id1", 1);
            x.Where().Or.Value("table_a.id").Equal(":id2").AddParameter(":id2", 2);
        });

        q.Where().Value("table_a", "id").Equal("table_b", "id");
        q.Where().Value("table_a", "id").Equal(":sub_id").AddParameter(":sub_id", 2);
        q.Where().Value("table_a", "id").IsNull();
        q.Where().Value("table_a", "id").IsNotNull();

        q.Where().Group(x =>
        {
            x.Where().Or.Value("table_a.id").Equal(":id1").AddParameter(":id1", 1);
            x.Where().Or.Value("table_a.id").Equal(":id2").AddParameter(":id2", 2);
        });

        q.Where().Exists(() =>
        {
            var x = new SelectQuery();
            x.From("table_x", "x");
            x.SelectAll();
            x.Where().Value("x.id").Equal("table_a.id");
            return x;
        });

        q.Where().Not.Exists(() =>
        {
            var x = new SelectQuery();
            x.From("table_x", "x");
            x.SelectAll();
            x.Where().Value("x.id").Equal("table_a.id");
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
```

### CTE
```cs
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
```

### Table creation query
```cs
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");

        var tq = new CreateTableQuery() { SelectQuery = q, TableName = "tmp" };

        var text = tq.ToQuery().CommandText;
        var expect = @"create table tmp
as
select table_a.*
from table_a";

        Assert.Equal(expect, text);
    }
```

### View creation query
```cs
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");

        var tq = new CreateViewQuery() { SelectQuery = q, ViewName = "tmp" };

        var text = tq.ToQuery().CommandText;
        var expect = @"create view tmp
as
select table_a.*
from table_a";

        Assert.Equal(expect, text);
    }
```

### Insert query
```cs
    [Fact]
    public void ColumnNameAlias()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(table_a, "id", "index_value");

        var tq = new InsertQuery() { SelectQuery = q, TableName = "table_b" };

        var text = tq.ToQuery().CommandText;
        var expect = @"insert into table_b(index_value)
select a.id as index_value
from table_a as a";

        Assert.Equal(expect, text);
    }
````

### Parse(version 0.4 or later)
You can parse handwritten SQL and use it as SqModel.
Table joins and inline queries can be parsed, but there are patterns that cannot be parsed (ex.group by, order by).

```c
    [Fact]
    public void Simple()
    {
        using var p = new Parser(@"select a.column_1 as col1, a.column_2 as col2 from table_a as a");
        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select a.column_1 as col1, a.column_2 as col2
from table_a as a";
        Assert.Equal(expect, text);

        Assert.Equal("a", q.SelectClause.ColumnClauses[0].TableName);
        Assert.Equal("column_1", q.SelectClause.ColumnClauses[0].Value);
        Assert.Equal("col1", q.SelectClause.ColumnClauses[0].AliasName);

        Assert.Equal("table_a", q.FromClause.TableName);
        Assert.Equal("a", q.FromClause.AliasName);
    }
```
