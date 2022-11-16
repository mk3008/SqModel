using SqModel;
using SqModel.Analysis;
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
    public void Default()
    {
        using var p = new SqlParser(@"select a.* from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.*
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void UpperCase()
    {
        var sq = SqlParser.Parse(@"SELECT A.Id FROM A WHERE A.Id = 1");

        var text = sq.ToQuery().CommandText;
        var expect = @"select
    A.Id
from A
where
    A.Id = 1";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void Parameter()
    {
        using var p = new SqlParser(@"select :prm as val from a");
        p.Logger = (x) => Output.WriteLine(x);

        var sq = p.ParseSelectQuery();
        sq.Parameters ??= new();
        sq.Parameters.Add(":prm", 1);

        var q = sq.ToQuery();
        var text = sq.ToQuery().CommandText;
        var expect = @"select
    :prm as val
from a";
        Assert.Equal(expect, text);
        Assert.Equal(1, q.Parameters[":prm"]);
    }

    [Fact]
    public void Simple()
    {
        using var p = new SqlParser(@"select a.column_1 as col1, a.column_2 as col2 from table_a as a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.column_1 as col1
    , a.column_2 as col2
from table_a as a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void Simple2()
    {
        using var p = new SqlParser(@"select(select b.value from b) as b_value from table_a as a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    (select b.value from b) as b_value
from table_a as a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void PushAndSelect_OldStyle()
    {
        using var p = new SqlParser(@"select table_a.id, table_a.name from table_a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        q = q.PushToCommonTable("a");
        var a = q.From("a");
        var cte_a = q.GetCommonTables().Where(x => x.Name == "a").First();
        var cols = cte_a.Query.GetSelectItems().Select(x => x.Name).ToList();
        cols.ForEach(x => q.Select.Add().Column("a", x));

        var text = q.ToQuery().CommandText;
        var expect = @"with
a as (
    select
        table_a.id
        , table_a.name
    from table_a
)
select
    a.id
    , a.name
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void PushAndSelect_NewStyle()
    {
        using var p = new SqlParser(@"select table_a.id, table_a.name from table_a");
        p.Logger = (x) => Output.WriteLine(x);

        var cteName = "common_a";
        var q = p.ParseSelectQuery();
        q = q.PushToCommonTable(cteName);

        var a = q.From(q.GetCommonTable(cteName)).As("a");
        q.SelectAll(a);

        var text = q.ToQuery().CommandText;
        var expect = @"with
common_a as (
    select
        table_a.id
        , table_a.name
    from table_a
)
select
    a.id
    , a.name
from common_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void OrderBy()
    {
        using var p = new SqlParser(@"select a.id, a.name, a.price from a order by a.id, a.name, a.price");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.id
    , a.name
    , a.price
from a
order by
    a.id
    , a.name
    , a.price";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void GroupBy()
    {
        using var p = new SqlParser(@"select a.id, a.sub_id, sum(a.price) as price from a group by a.id, a.sub_id");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.id
    , a.sub_id
    , sum(a.price) as price
from a
group by
    a.id
    , a.sub_id";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void Expression_noalias()
    {
        using var p = new SqlParser(@"select
    a.col1 * 1.23 c
from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.col1 * 1.23 as c
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void Expression()
    {
        using var p = new SqlParser(@"select
    1.23 * a.col1
    , 1.23 * a.col1 c
    , 1.23 * a.col1 as c
    , a.col1 * 1.23
    , a.col1 * 1.23 c
    , a.col1 * 1.23 as c
    , trunc(a.col1 * a.col2 * a.col3) as val
from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    1.23 * a.col1
    , 1.23 * a.col1 as c
    , 1.23 * a.col1 as c
    , a.col1 * 1.23
    , a.col1 * 1.23 as c
    , a.col1 * 1.23 as c
    , trunc(a.col1 * a.col2 * a.col3) as val
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void Brackets()
    {
        using var p = new SqlParser(@"select * from a where ((a.c2 = 2) or (a.c3 = 3)) and a.c1 = 1");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    *
from a
where
    ((a.c2 = 2) or (a.c3 = 3)) and a.c1 = 1";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void WindowFunction()
    {
        using var p = new SqlParser(@"select row_number() over (partition by a.name order by a.id) as row_num from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    row_number() over(partition by a.name order by a.id) as row_num
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void WindowFunction_nospace()
    {
        using var p = new SqlParser(@"select row_number() over(partition by a.name order by a.id) as row_num from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    row_number() over(partition by a.name order by a.id) as row_num
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void TypeConvert()
    {
        using var p = new SqlParser(@"select (1+1)::text as v1 from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    (1+1)::text as v1
from a";
        Assert.Equal(expect, text);
    }


    [Fact]
    public void ExpressionAndTypeConvert()
    {
        using var p = new SqlParser(@"select to_char(a.col1, 'yyyy')::int as v1 from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    to_char(a.col1, 'yyyy')::int as v1
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void Pipe()
    {
        using var p = new SqlParser(@"select a.val1 || a.val2 as text from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.val1 || a.val2 as text
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void CasePipe()
    {
        using var p = new SqlParser(@"select case when 1=1 then '1' else '2' end || 'a' as text from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    case when 1 = 1 then '1' else '2' end || 'a' as text
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void PipeCase()
    {
        using var p = new SqlParser(@"select a.txt1 || case when 1=1 then 1 else 2 end as text from a");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.txt1 || case when 1 = 1 then 1 else 2 end as text
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void CaseWhenBool()
    {
        using var p = new SqlParser(@"select case when true then true else false end");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    case when true then true else false end";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void NullTest()
    {
        using var p = new SqlParser(@"select * from a where a.id is not null and a.id is null");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    *
from a
where
    a.id is not null and a.id is null";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void CaseWhenTest()
    {
        using var p = new SqlParser(@"select case when 1=1 then 1 end val");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    case when 1 = 1 then 1 end as val";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void CaseWhenElseTest()
    {
        using var p = new SqlParser(@"select case when 1=1 then 1 else 2 end val");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    case when 1 = 1 then 1 else 2 end as val";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void CaseTest()
    {
        using var p = new SqlParser(@"select case 1 when 1 then 1 end val");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    case 1 when 1 then 1 end as val";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void CaseElseTest()
    {
        using var p = new SqlParser(@"select case 1 when 1 then 1 else 2 end val");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    case 1 when 1 then 1 else 2 end as val";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void TypeConvertWithBracket()
    {
        var sq = SqlParser.Parse("select 3.1415::numeric(8,2) as val1");
        var text = sq.ToQuery().CommandText;
        var expect = @"select
    3.1415::numeric(8,2) as val1";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void TypeConvertWithBracketColumn()
    {
        var sq = SqlParser.Parse("select a.val1::numeric(8,2) as val1 from a");
        var text = sq.ToQuery().CommandText;
        var expect = @"select
    a.val1::numeric(8,2) as val1
from a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void BooleanColumn()
    {
        var sq = SqlParser.Parse("select 1 + 1 = 2 calc");
        var text = sq.ToQuery().CommandText;
        var expect = @"select
    1 + 1 = 2 as calc";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void BooleanColumn_operator()
    {
        var sq = SqlParser.Parse(@"select 
  1 + 1 = 2 and 2 + 2 = 4 and 3 + 3 = 6 calc1
, 1 + 1 = 2 or  2 * 2 = 2 or  3 + 3 = 3 calc2");
        var text = sq.ToQuery().CommandText;
        var expect = @"select
    1 + 1 = 2 and 2 + 2 = 4 and 3 + 3 = 6 calc1
    , 1 + 1 = 2 or 2 * 2 = 2 or 3 + 3 = 3 calc2";
        Assert.Equal(expect, text);
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
    or a.column_2 = 2
");
        p.Logger = (x) => Output.WriteLine(x);

        var q = p.ParseSelectQuery();

        var text = q.ToQuery().CommandText;
        var expect = @"select
    a.column_1 as col1
    , a.column_2 as col2
    , ((1+2) * 3) as col3
    , (select b.value from b) as b_value
    , ' comment('')comment ' as comment
from table_a as a
inner join table_c as c on a.column_1 = c.column_1
left join table_d as d on a.column_2 = d.column_2 and a.column_3 = d.column_3
left join table_e as e on a.column_4 = e.column_4
where
    a.column_1 = 1 or a.column_2 = 2";

        Assert.Equal(expect, text);

        Assert.Equal(5, q.Select.Collection.Count);

        ////ColumnClauses(Column)
        //Assert.Equal("a", q.SelectClause.Collection[0].TableName);
        //Assert.Equal("column_1", q.SelectClause.Collection[0].Value);
        //Assert.Equal("col1", q.SelectClause.Collection[0].AliasName);

        ////ColumnClauses(Value)
        //Assert.Equal("((1+2) * 3)", q.SelectClause.Collection[2].Value);
        //Assert.Equal("col3", q.SelectClause.Collection[2].AliasName);

        //ColumnClauses(InlineQuery)
        //Assert.Equal("b", q.SelectClause.Collection[3].InlineQuery?.FromClause.TableName);
        //Assert.Equal("b", q.SelectClause.Collection[3].InlineQuery?.SelectClause.ColumnClauses[0].TableName);
        //Assert.Equal("value", q.SelectClause.Collection[3].InlineQuery?.SelectClause.ColumnClauses[0].Value);

        ////FromClause
        //Assert.Equal("table_a", q.FromClause.TableName);
        //Assert.Equal("a", q.FromClause.AliasName);
        //Assert.Equal(3, q.FromClause.SubTableClauses.Count);

        ////SubTableClauses
        //Assert.Equal(RelationTypes.Inner, q.FromClause.SubTableClauses[0].RelationType);
        //Assert.Equal("table_c", q.FromClause.SubTableClauses[0].TableName);
        //Assert.Equal("c", q.FromClause.SubTableClauses[0].AliasName);
        //Assert.Single(q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup);
        //Assert.Equal("a", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.Source?.TableName);
        //Assert.Equal("column_1", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.Source?.Value);
        //Assert.Equal("=", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.ValueConjunction?.Sign);
        //Assert.Equal("c", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.ValueConjunction?.Destination.TableName);
        //Assert.Equal("column_1", q.FromClause.SubTableClauses[0].RelationClause.ConditionGroup?[0].Condition?.ValueConjunction?.Destination.Value);

        ////WhereClauses
        //Assert.Equal(2, q.WhereClause.Container.ConditionGroup?.Count);
        //Assert.Equal("or", q.WhereClause.Container.ConditionGroup?[1].Operator);
        //Assert.Equal("a", q.WhereClause.Container.ConditionGroup?[1].Condition?.Source?.TableName);
        //Assert.Equal("column_2", q.WhereClause.Container.ConditionGroup?[1].Condition?.Source?.Value);
        //Assert.Equal("=", q.WhereClause.Container.ConditionGroup?[1].Condition?.ValueConjunction?.Sign);
        //Assert.Equal("2", q.WhereClause.Container.ConditionGroup?[1].Condition?.ValueConjunction?.Destination.Value);
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
