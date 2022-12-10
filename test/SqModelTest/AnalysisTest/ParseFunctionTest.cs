using SqModel.Analysis;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseFunctionTest
{
    public ParseFunctionTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Coalesce()
    {
        var sql = @"select
    coalesce(a.name_2, a.name_1) as name
from table_a";
        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void Like()
    {
        var sql = @"select
    *
from table_a
where
    a.name like :name";
        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void NotLike()
    {
        var sql = @"select
    *
from table_a
where
    a.name not like :name";
        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void In()
    {
        var sql = @"select
    *
from table_a
where
    a.id in (1, 2, 3)";
        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void Any()
    {
        var sql = @"select
    *
from table_a
where
    a.id = any(1, 2, 3)";

        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void Concat()
    {
        var sql = @"select
    concat('a', 'b', 'c') as text
from table_a";

        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void StringJoin()
    {
        var sql = @"select
    'a' || 'b' || 'c' as text
from table_a";

        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void Now()
    {
        //NoArgument
        var sql = @"select
    now() as date1
from table_a";

        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }

    [Fact]
    public void RowNumber()
    {
        var sql = @"select
    row_number() over(partition by name1, name2 order by id, sub_id) as rowid
from table_a";

        var sq = SqlParser.Parse(sql);

        Assert.Equal(sql, sq.ToQuery().CommandText);
    }
}