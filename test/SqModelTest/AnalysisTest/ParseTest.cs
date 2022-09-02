using SqModel;
using SqModel.Clause;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ParseTest
{
    public ParseTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Simple()
    {
        using var p = new SqlParser(@"select a.column_1 as col1, a.column_2 as col2 from table_a as a");
        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select a.column_1 as col1, a.column_2 as col2
from table_a as a";
        Assert.Equal(expect, text);

        Assert.Equal("a", q.SelectClause.Collection[0].TableName);
        Assert.Equal("column_1", q.SelectClause.Collection[0].Value);
        Assert.Equal("col1", q.SelectClause.Collection[0].AliasName);

        Assert.Equal("table_a", q.FromClause.TableName);
        Assert.Equal("a", q.FromClause.AliasName);
    }

    [Fact]
    public void Full()
    {
        using var p = new SqlParser(@"select
    a.column_1 as col1
    , a.column_2 as col2
    , ((1+2) * 3) as col3
    , (select b.value from b) as b_value
    , ' comment('')comment ' as comment /* prefix /* nest */ sufix */
from 
    table_a as a
    inner join table_c as c on a.column_1 = c.column_1
    left outer join table_d as d on a.column_2 = d.column_2 and a.column_3 = d.column_3
    left join table_e as e on a.column_4 = e.column_4
where
    a.column_1 = 1
    or  a.column_2 = 2
");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();

        var text = q.ToQuery().CommandText;
        var expect = @"select a.column_1 as col1, a.column_2 as col2, ((1+2) * 3) as col3, (select b.value from b) as b_value, ' comment('')comment ' as comment
from table_a as a
inner join table_c as c on a.column_1 = c.column_1
left join table_d as d on a.column_2 = d.column_2 and a.column_3 = d.column_3
left join table_e as e on a.column_4 = e.column_4
where
    a.column_1 = 1 or a.column_2 = 2";

        Assert.Equal(expect, text);

        Assert.Equal(5, q.SelectClause.Collection.Count);

        //ColumnClauses(Column)
        Assert.Equal("a", q.SelectClause.Collection[0].TableName);
        Assert.Equal("column_1", q.SelectClause.Collection[0].Value);
        Assert.Equal("col1", q.SelectClause.Collection[0].AliasName);

        //ColumnClauses(Value)
        Assert.Equal("((1+2) * 3)", q.SelectClause.Collection[2].Value);
        Assert.Equal("col3", q.SelectClause.Collection[2].AliasName);

        //ColumnClauses(InlineQuery)
        Assert.Equal("b", q.SelectClause.Collection[3].InlineQuery?.FromClause.TableName);
        Assert.Equal("b", q.SelectClause.Collection[3].InlineQuery?.SelectClause.ColumnClauses[0].TableName);
        Assert.Equal("value", q.SelectClause.Collection[3].InlineQuery?.SelectClause.ColumnClauses[0].Value);

        //FromClause
        Assert.Equal("table_a", q.FromClause.TableName);
        Assert.Equal("a", q.FromClause.AliasName);
        Assert.Equal(3, q.FromClause.SubTableClauses.Count);

        //SubTableClauses
        Assert.Equal(RelationTypes.Inner, q.FromClause.SubTableClauses[0].RelationType);
        Assert.Equal("table_c", q.FromClause.SubTableClauses[0].TableName);
        Assert.Equal("c", q.FromClause.SubTableClauses[0].AliasName);
        Assert.Single(q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup);
        Assert.Equal("a", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.Source?.TableName);
        Assert.Equal("column_1", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.Source?.Value);
        Assert.Equal("=", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.ValueConjunction?.Sign);
        Assert.Equal("c", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.ValueConjunction?.Destination.TableName);
        Assert.Equal("column_1", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.ValueConjunction?.Destination.Value);

        //WhereClauses
        Assert.Equal(2, q.WhereClause.Container.ConditionGroup?.Count);
        Assert.Equal("or", q.WhereClause.Container.ConditionGroup?[1].Operator);
        Assert.Equal("a", q.WhereClause.Container.ConditionGroup?[1].Condition?.Source?.TableName);
        Assert.Equal("column_2", q.WhereClause.Container.ConditionGroup?[1].Condition?.Source?.Value);
        Assert.Equal("=", q.WhereClause.Container.ConditionGroup?[1].Condition?.ValueConjunction?.Sign);
        Assert.Equal("2", q.WhereClause.Container.ConditionGroup?[1].Condition?.ValueConjunction?.Destination.Value);
    }

    //    [Fact]
    //    public void Simple()
    //    {
    //        using var p = new SqlParser(@"select
    //    a.*
    //    , *
    //    , a.column_1 as col1
    //    , a.column_2 col2 
    //    , a.column_3 
    //    , column_11 as col11
    //    , column_12 col12 
    //    , column_13 
    //    , (1*2) * 3 as calc1
    //    , (1*2) * 3 calc2
    //    , (select 1 from table_b) as val1
    //from 
    //    table_a as a");
    //        p.Logger = (x) => Output.WriteLine(x);

    //        var q = p.ParseSelectQuery();
    //    }

    //    [Fact]
    //    public void SimpleWith()
    //    {
    //        using var p = new SqlParser(@"
    //with
    //a as (
    //    select
    //        a.column_1 as col1
    //        , a.column_2 col2 
    //        , a.column_3 
    //    from 
    //        table_a
    //), 
    //b as (
    //    select
    //        column_11 as col11
    //        , column_12 col12 
    //        , column_13  
    //    from 
    //        table_b
    //)
    //select * from a");
    //        p.Logger = (x) => Output.WriteLine(x);

    //        var q = p.ParseSelectQuery();
    //    }
}
