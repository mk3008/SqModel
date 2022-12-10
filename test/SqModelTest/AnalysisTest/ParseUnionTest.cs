using SqModel.Analysis;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseUnionTest
{

    public ParseUnionTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        var sql = @"select
    1
union all
select
    2
union
select
    *
from table_a";
        var clause = SqlParser.Parse(sql);

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }
}
