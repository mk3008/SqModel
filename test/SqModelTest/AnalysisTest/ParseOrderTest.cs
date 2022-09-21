using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseOrderTest
{
    public ParseOrderTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void OrderBy()
    {
        /// inner join table_b as b on ...
        var sql = "order by a.id, 1, b.id";
        var clause = OrderClauseParser.Parse(sql);

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void Default()
    {
        /// inner join table_b as b on ...
        var sql = @"select
    *
from table_a
where
    1 = 1
order by a.id, 1, a.name";
        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }
}