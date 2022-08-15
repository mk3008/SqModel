# SqModel
A lightweight library that allows you to easily build Select queries.
You can also parse handwritten Sql.

## Demo
```cs
var q = new SelectQuery();
var table_a = q.From("table_a");
q.Select(table_a, "*");
q.Where.And(table_a, "id", ":id", 1);

var acutal = q.ToQuery();
var expect = @"select table_a.*
from table_a
where
    table_a.id = :id";

Assert.Equal(expect, acutal.CommandText);
Assert.Single(acutal.Parameters);
Assert.Equal(1, acutal.Parameters[":id"]);
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

### Table join
```cs
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
```

### Subquery
```cs
    [Fact]
    public void Valiable()
    {
        var q = new SelectQuery();
        var a = q.From(sq =>
        {
            var t = sq.From("table_a");
            sq.Select(t, "*");
            sq.Select(":val1", "val1").AddParameter(":val1", 1);
        }
        , "a");

        var b = a.InnerJoin(sq =>
        {
            var t = sq.From("table_b");
            sq.Select(t, "*");
            sq.Select(":val2", "val2").AddParameter(":val2", 2);
        }, "b", new() { "table_a_id" });

        q.Select(a, "*");
        q.Select(b, "*");

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select a.*, b.*
from (
    select table_a.*, :val1 as val1
    from table_a
) as a
inner join (
    select table_b.*, :val2 as val2
    from table_b
) as b on a.table_a_id = b.table_a_id";

        Assert.Equal(expect, text);
        Assert.Equal(1, actual.Parameters[":val1"]);
        Assert.Equal(2, actual.Parameters[":val2"]);
    }
```

### Extraction condition
```cs
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where.And(table_a, "id", ":id", 1);

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

### Extraction condition(group)
```cs
    [Fact]
    public void OperatorOr()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where.And(g =>
        {
            g.Or("table_a.id = :id1").AddParameter(":id1", 1);
            g.Or("table_a.id = :id2").AddParameter(":id2", 2);
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

### Use only extraction conditions
```cs
    [Fact]
    public void WhereOnly()
    {
        var q = new SelectQuery();
        q.Where.And("table_a.sub_id = :sub_id").AddParameter(":sub_id", 2);
        q.Where.And(g =>
        {
            g.Or("table_a.id = :id1").AddParameter(":id1", 1);
            g.Or("table_a.id = :id2").AddParameter(":id2", 2);
        });

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
```

### CTE
```cs
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        q.With(x =>
        {
            var t = x.From("table_a");
            x.Select(t, "*");
        }, "a");

        var a = q.From("a");
        q.Select(a, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"with
a as (
    select table_a.*
    from table_a
)
select a.*
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
