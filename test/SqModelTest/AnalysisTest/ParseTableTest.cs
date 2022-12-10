using SqModel.Analysis;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseTableTest
{
    public ParseTableTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void FromTable()
    {
        /// inner join table_b as b on ...
        var sql = "from table_a as a";
        var clause = FromClauseParser.Parse(sql);

        Assert.Equal("table_a", clause.TableName);
        Assert.Equal("a", clause.AliasName);
        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void JoinTable()
    {
        /// inner join table_b as b on ...
        var text = @"from table_a as a
inner join table_b as b on a.column_1 = b.column_1 and a.column_2 and b.column_2";
        var clause = FromClauseParser.Parse(text);

        var sql = clause.ToQuery().CommandText;

        Assert.Equal(text, sql);
    }

    [Fact]
    public void Default()
    {
        /// inner join table_b as b on ...
        var sql = @"from table_a as a
inner join table_b as b on a.column_1 = b.column_1 and a.column_2 = b.column_2";
        var clause = FromClauseParser.Parse(sql);

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }


    [Fact]
    public void ManyJoin()
    {
        /// inner join table_b as b on ...
        var sql = @"from table_a as a
inner join table_b as b on a.column_1 = b.column_1 and a.column_2 and b.column_2
left join table_c as c on b.column_3 = c.column_3
right join table_d as d on c.column_4 = c.column4
cross join table_e as e";
        var clause = FromClauseParser.Parse(sql);

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void SubQuery()
    {
        /// inner join table_b as b on ...
        var sql = @"from (
    select
        *
    from table_a
) as a";
        var clause = FromClauseParser.Parse(sql);

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void SubQueryMany()
    {
        /// inner join table_b as b on ...
        var sql = @"from (
    select
        *
    from table_a
) as a
inner join (
    select
        *
    from table_b
) as b on a.column_1 = b.column_1";
        var clause = FromClauseParser.Parse(sql);

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void AliasOmit()
    {
        var sql = @"from table_a
inner join table_b on table_a.column_1 = table_b.column_1";
        var clause = FromClauseParser.Parse(sql);

        var text = clause.ToQuery().CommandText;
        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void Schema()
    {
        var sql = @"from schema.table as t";
        var clause = FromClauseParser.Parse(sql);

        var text = clause.ToQuery().CommandText;
        Assert.Equal(sql, clause.ToQuery().CommandText);
    }
}
