using SqModel.Analysis;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseWhereTest
{
    public ParseWhereTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        var condition = "where a.column_1 = 1 or a.column_2 = 2";
        var clause = WhereClauseParser.Parse(condition);
        var q = clause.ToQuery();
        var expect = @"where
    a.column_1 = 1 or a.column_2 = 2";
        Assert.Equal(expect, q.CommandText);
    }


    [Fact]
    public void Exists()
    {
        var condition = "where exists (select * from table_b as b where b.id = a.id)";
        var clause = WhereClauseParser.Parse(condition);
        //clause.IsOneLineFormat = true;
        //clause.ConditionGroup.IsOneLineFormat = true;
        var q = clause.ToQuery();
        var expect = @"where
    exists (select * from table_b as b where b.id = a.id)";
        Assert.Equal(expect, q.CommandText);
    }

    [Fact]
    public void NotExists()
    {
        var condition = "where not exists (select * from table_b as b where b.id = a.id)";
        var clause = WhereClauseParser.Parse(condition);
        var q = clause.ToQuery();
        var expect = @"where
    not exists (select * from table_b as b where b.id = a.id)";
        Assert.Equal(expect, q.CommandText);
    }

    [Fact]
    public void Group()
    {
        var condition = "where (a.id = 1 or a.id = 2) and (a.value = 1 or (a.id = 3 and a.id = 4))";
        var clause = WhereClauseParser.Parse(condition);
        var q = clause.ToQuery();
        var expect = @"where
    (a.id = 1 or a.id = 2) and (a.value = 1 or (a.id = 3 and a.id = 4))";
        Assert.Equal(expect, q.CommandText);
    }
}
