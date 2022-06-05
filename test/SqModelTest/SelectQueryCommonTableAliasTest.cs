using SqModel;
using Xunit;

namespace SqModelTest;

public class SelectQueryCommonTableAliasTest
{
    [Fact]
    public void Default()
    {
        var sq = new SelectQuery();

        var t1 = new SelectQuery();
        t1.Table.TableName = "a";

        var sql = @"select
    column
from 
    table_x";

        sq.WithClause.Add(sql, "a");
        sq.FromClause = t1;

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
    public void ManyCTE()
    {
        var sq = new SelectQuery();

        var t1 = new SelectQuery();
        t1.Table.TableName = "x";
        var t2 = new SelectQuery();
        t2.Table.TableName = "y";

        var sql1 = @"select
    column_x
from 
    table_x";

        var sql2 = @"select
    column_x, column_y
from 
    table_y";

        sq.WithClause.Add(sql1, t1);
        sq.WithClause.Add(sql2, t2);
        sq.FromClause = t1;

        var tr = sq.JoinTableRelationClause.Add(t1, t2);
        tr.AddCondition("column_x");

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
