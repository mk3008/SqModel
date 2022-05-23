using SqModel;
using Xunit;

namespace SqModelTest;

public class SelectQueryCommonTableAliasTest
{
    [Fact]
    public void Default()
    {
        var sq = new SelectQuery();

        var t1 = new TableAlias();
        t1.Table.TableName = "a";

        var sql = @"select
    column
from 
    table_x";

        sq.AddCommonTableAliases(sql, "a");
        sq.Root = t1;

        var text = sq.ToQuery().CommandText;
        var expect = @"with
a as (
    select
        column
    from 
        table_x
)
select a.*
from a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Mulit()
    {
        var sq = new SelectQuery();

        var t1 = new TableAlias();
        t1.Table.TableName = "x";
        var t2 = new TableAlias();
        t2.Table.TableName = "y";

        var sql1 = @"select
    column_x
from 
    table_x";

        var sql2 = @"select
    column_x, column_y
from 
    table_y";

        sq.AddCommonTableAlias(sql1, t1);
        sq.AddCommonTableAlias(sql2, t2);
        sq.Root = t1;

        var tr = sq.AddTableRelation(t1, t2);
        tr.AddColumnRelation("column_x");

        var text = sq.ToQuery().CommandText;
        var expect = @"with
x as (
    select
        column_x
    from 
        table_x
),
y as (
    select
        column_x, column_y
    from 
        table_y
)
select x.*, y.*
from x
inner join y on x.column_x = y.column_x";

        Assert.Equal(expect, text);
    }

}
