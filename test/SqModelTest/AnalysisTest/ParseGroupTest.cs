using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseGroupTest
{
    public ParseGroupTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void GroupBy()
    {
        /// inner join table_b as b on ...
        var sql = "group by a.id, 1, b.id";
        var clause = GroupClauseParser.Parse(sql);

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void Default()
    {
        /// inner join table_b as b on ...
        var sql = @"select 1
from table_a
where
    1 = 1
group by a.id, 1, a.name";
        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void SumTest()
    {
        /// inner join table_b as b on ...
        var sql = @"select sum(a.price) as price
from table_a
where
    1 = 1
group by a.id, 1, a.name";
        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void CountTest()
    {
        /// inner join table_b as b on ...
        var sql = @"select count(*) as cnt
from table_a
where
    1 = 1
group by a.id, 1, a.name";
        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }
}