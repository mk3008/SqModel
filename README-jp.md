# 概要
SqModel は Selectクエリをモデル化し、簡易に組み上げることができる軽量ライブラリです。

## デモ
```
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
SQL構文に寄せたメソッド名（From、Select、Join、Where etc）。
パラメータクエリに対応。
テーブル別名、列別名に対応。
CTE（Common Table Expression）に対応。
控えめなSQL整形。

## 制約
SQL構文チェック機能はありません。

## 実行環境
.NET6

## サンプル
### 単一テーブル選択
```
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
```
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

```
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
```
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

### 抽出条件
```
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

### CTE
```
    [Fact]
    public void Default()
    {
        var sub = new SelectQuery();
        var table_a = sub.From("table_a");
        sub.Select(table_a, "*");

        var q = new SelectQuery();
        q.With.Add(sub, "a");
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
