using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ParseConditionGroupTest
{
    public ParseConditionGroupTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        /// inner join table_b as b on ...
        var relationSql = "a.id1 = b.id1 and a.id2 = b.id2 and a.id3 = b.id3";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseOperatorContainer();
        clause.IsRoot = true;
        var q = clause.ToQuery();
        Assert.Equal(relationSql, q.CommandText);
    }

    [Fact]
    public void LogicalOperatorMix()
    {
        /// inner join table_b as b on ...
        var relationSql = "a.id1 = b.id1 and a.id2 = b.id2 or a.id3 = b.id3";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseOperatorContainer();
        clause.IsRoot = true;
        var q = clause.ToQuery();
        Assert.Equal("a.id1 = b.id1 and a.id2 = b.id2 or a.id3 = b.id3", q.CommandText);
    }

    [Fact]
    public void GroupCondition()
    {
        /// inner join table_b as b on ...
        var relationSql = "a.id3 = b.id3 or (a.id1 = b.id1 and a.id2 = b.id2)";
        using var p = new Parser(relationSql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseOperatorContainer();
        clause.IsRoot = true;
        var q = clause.ToQuery();
        Assert.Equal("a.id3 = b.id3 or (a.id1 = b.id1 and a.id2 = b.id2)", q.CommandText);
    }
}
