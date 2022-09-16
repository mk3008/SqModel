using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    a.column_1 = 1
    or a.column_2 = 2";
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
}
