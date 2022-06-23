# 概要
SqModel は Selectクエリをモデル化し、簡易に組み上げることができる軽量ライブラリです。

## デモ
```cs
var q = new SelectQuery();
var table_a = q.From("table_a");
q.Select(table_a, "*");
q.Where(table_a, "id", ":id", 1);

var acutal = q.ToQuery();
var expect = @"select table_a.*
from table_a
where
    table_a.id = :id";

Assert.Equal(expect, acutal.CommandText);
Assert.Single(acutal.Parameters);
Assert.Equal(1, acutal.Parameters[":id"]);
````

## 特徴
- SQL構文に寄せたメソッド名（From、Select、Join、Where etc）。
- テーブル別名、列別名に対応。
- パラメータクエリに対応。
- サブクエリに対応。
- CTE（Common Table Expression）に対応。
- テーブル作成クエリに対応。
- ビュー作成クエリに対応。
- 控えめなSQL整形。

## 制約
SQL構文チェック機能はありません。

## 実行環境
.NET6

## サンプル
### 単一テーブル選択
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
````

### 列の取得
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

### 変数を使用したパラメータクエリ

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

### 結合
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

### サブクエリ
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

### 抽出条件
```cs
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where(table_a, "id", ":id", 1);

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

### 抽出条件（グループ）
```cs
    [Fact]
    public void OperatorOr()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");
        q.Where(g =>
        {
            g.Where("table_a.id = :id1").AddParameter(":id1", 1);
            g.Where("table_a.id = :id2").AddParameter(":id2", 2);
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

### 抽出条件だけを使う
```cs
    [Fact]
    public void WhereOnly()
    {
        var q = new SelectQuery();
        q.Where(g =>
        {
            g.Where("table_a.id = :id1").AddParameter(":id1", 1);
            g.Where("table_a.id = :id2").AddParameter(":id2", 2);
        });
        q.Where("table_a.sub_id = :sub_id").AddParameter(":sub_id", 2);

        var acutal = q.WhereClause.ToQuery();
        var expect = @"where
    (table_a.id = :id1 or table_a.id = :id2)
    and table_a.sub_id = :sub_id";

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

### テーブル作成
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

### ビュー作成
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
