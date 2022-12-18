using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Carbunql.Analysis;
using Carbunql.Core;
using Carbunql.Core.Extensions;

class Program
{
    static void Main(string[] args) => BenchmarkRunner.Run<Test>();
}

public class Test
{
    public static string Sql = @"select
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


    private SqModel.SelectQuery sqmodel = SqModel.Analysis.SqlParser.Parse(Sql);

    [Benchmark]
    public string SqModelParse()
    {
        var sq = SqModel.Analysis.SqlParser.Parse(Sql);
        return "success";// sq.ToQuery().CommandText;
    }

    [Benchmark]
    public string SqModelString()
    {
        return sqmodel.ToQuery().CommandText;
    }

    private QueryBase carbunql = QueryParser.Parse(Sql);

    [Benchmark]
    public string CarbunqlParse()
    {
        var sq = QueryParser.Parse(Sql);
        return "success";// sq.GetTokens().ToString(" ");
    }

    [Benchmark]
    public string CarbunqlString()
    {
        return carbunql.GetTokens(null).ToString(" ");
    }
}