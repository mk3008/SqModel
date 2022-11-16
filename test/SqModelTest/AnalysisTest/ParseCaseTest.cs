using SqModel;
using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseCaseTest
{
    public ParseCaseTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void CaseWhenTest()
    {
        using var p = new SqlParser(@"case when a.col = 1 then 1 when a.col = 2 then 2 else 3 end");
        var q = CaseExpressionParser.Parse(p);
        var text = q.ToQuery().CommandText;
        var expect = @"case when a.col = 1 then 1 when a.col = 2 then 2 else 3 end";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void CaseTest()
    {
        using var p = new SqlParser(@"case a.col when 1 then 1 when 2 then 2 else 3 end");
        var q = CaseExpressionParser.Parse(p);
        var text = q.ToQuery().CommandText;
        var expect = @"case a.col when 1 then 1 when 2 then 2 else 3 end";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void NestTest()
    {
        using var p = new SqlParser(@"
case a.col 
    when 1 then 
        case b.col 
            when 10 then 10 
            else 20
        end
    when 2 then 2
    else 3 
end");
        var q = CaseExpressionParser.Parse(p);
        var text = q.ToQuery().CommandText;
        var expect = @"case a.col when 1 then case b.col when 10 then 10 else 20 end when 2 then 2 else 3 end";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void SelectTest()
    {
        using var p = new SqlParser(@"select case when a.col = 1 then 1 else 2 end as val from table_a as a");
        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    case when a.col = 1 then 1 else 2 end as val
from table_a as a";
        Assert.Equal(expect, text);
    }

    [Fact]
    public void AndOrTest()
    {
        using var p = new SqlParser(@"select case when 1 = 1 and 2 = 2 then 1 when 3 = 3 or 4 = 4 then 2 end as val");
        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    case when 1 = 1 and 2 = 2 then 1 when 3 = 3 or 4 = 4 then 2 end as val";
        Assert.Equal(expect, text);
    }


    [Fact]
    public void WhereTest()
    {
        using var p = new SqlParser(@"select * from table_a as a where 
case when a.col = 1 then 1 else 2 end = 1");
        var q = p.ParseSelectQuery();
        var text = q.ToQuery().CommandText;
        var expect = @"select
    *
from table_a as a
where
    case when a.col = 1 then 1 else 2 end = 1";
        Assert.Equal(expect, text);
    }
}
