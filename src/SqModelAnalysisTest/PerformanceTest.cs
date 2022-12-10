using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace SqModelAnalysisTest;

public class PerformanceTest
{
    private readonly QueryCommandMonitor Monitor;

    public PerformanceTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Case1()
    {
        var text = @"SELECT View1.OrderDate,View1.Email,SUM(View1.TotalPayments) FROM dbo.View1 WHERE (View1.OrderStatus = 'Completed') GROUP BY View1.OrderDate,View1.Email HAVING (SUM(View1.TotalPayments) > 75)";
        var item = QueryParser.Parse(text);
        Monitor.Log(item);
    }

    [Fact]
    public void Case2()
    {
        var text = @"select a.id::text as id, '1'::int as v1, 1::text as v2, (1+1)::text as v3 from a";
        var item = QueryParser.Parse(text);
        Monitor.Log(item);
    }
}
