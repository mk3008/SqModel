using SqModel;
using Xunit;

namespace SqModelTest;

public class SelectQueryRelationTest
{
    [Fact]
    public void Default()
    {
        var sq = new SelectQuery();

        var t1 = new SelectQuery();
        t1.Table.TableName = "table_a";
        t1.Table.AliasName = "a";

        var t2 = new SelectQuery();
        t2.Table.TableName = "table_b";
        t2.Table.AliasName = "b";

        sq.FromClause = t1;
        sq.JoinTableRelationClause.Add(t1, t2).AddCondition("column");

        var text = sq.ToQuery().CommandText;
        var expect = @"select a.*, b.*
from table_a as a
inner join table_b as b on a.column = b.column";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void MultiColumnRelation()
    {
        var sq = new SelectQuery();

        var t1 = new SelectQuery();
        t1.Table.TableName = "table_a";
        t1.Table.AliasName = "a";

        var t2 = new SelectQuery();
        t2.Table.TableName = "table_b";
        t2.Table.AliasName = "b";

        sq.FromClause = t1;
        sq.JoinTableRelationClause.Add(t1, t2).AddCondition("col1").AddCondition("col2");

        var text = sq.ToQuery().CommandText;
        var expect = @"select a.*, b.*
from table_a as a
inner join table_b as b on a.col1 = b.col1 and a.col2 = b.col2";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void ManyTableRelation()
    {
        var sq = new SelectQuery();

        var t1 = new SelectQuery();
        t1.Table.TableName = "table_a";
        t1.Table.AliasName = "a";

        var t2 = new SelectQuery();
        t2.Table.TableName = "table_b";
        t2.Table.AliasName = "b";

         var t3 = new SelectQuery();
        t3.Table.TableName = "table_c";
        t3.Table.AliasName = "c";

        sq.FromClause = t1;
        sq.JoinTableRelationClause.Add(t1, t2).AddCondition("col1").AddCondition("col2");
        sq.JoinTableRelationClause.Add(t2, t3).AddCondition("colx").AddCondition("coly");

        var text = sq.ToQuery().CommandText;
        var expect = @"select a.*, b.*, c.*
from table_a as a
inner join table_b as b on a.col1 = b.col1 and a.col2 = b.col2
inner join table_c as c on b.colx = c.colx and b.coly = c.coly";

        Assert.Equal(expect, text);
    }
}
