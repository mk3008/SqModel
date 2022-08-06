using SqModel;
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

//    [Fact]
//    public void Full()
//    {
//        using var p = new Parser(@"
//with 
//a as (
//    select * from table_a
//), 
//b as (
//    select * from table_b
//)
//select
//    a.column_1 as col1
//    , a.column_2 as col2
//    , ((1+2) * 3) as col3
//    , (select b.value from b) as b_value
//    , ' comment('')comment ' as comment /* prefix /* nest */ sufix */
//from
//    table_a as a
//    inner join table_c as c on a.column_1 = c.column_1
//    left outer join table_d as d on a.column_2 = d.column_2 and a.column_3 = d.column_3
//    left join table_e as e on a.column_4 = e.column_4
//where
//    a.column_1 = 1
//order by 
//    a.column_1");
//        p.Logger = (x) => Output.WriteLine(x);

//        //var lst = p.ReadAllTokens().ToList();

//        var q = p.ParseSelectQuery();
//        var text = q.ToQuery().CommandText;
//    }

//    [Fact]
//    public void Simple()
//    {
//        using var p = new Parser(@"select
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
//        using var p = new Parser(@"
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
