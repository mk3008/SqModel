using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

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
        var condition = "1 = 1";
        using var p = new SqlParser(condition);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseWhereClause();
        var q = clause.ToQuery();
        var expect = @"where
    1 = 1";
        Assert.Equal(expect, q.CommandText);
    }


    [Fact]
    public void Exists()
    {
        var condition = "exists (select * from table_b as b where b.id = a.id)";
        using var p = new SqlParser(condition);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseWhereClause();
        var q = clause.ToQuery();
        var expect = @"where
    exists (
        select *
        from table_b as b
        where
            b.id = a.id
    )";
        Assert.Equal(expect, q.CommandText);
    }

    [Fact]
    public void NotExists()
    {
        var condition = "not exists (select * from table_b as b where b.id = a.id)";
        using var p = new SqlParser(condition);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseWhereClause();
        var q = clause.ToQuery();
        var expect = @"where
    not exists (
        select *
        from table_b as b
        where
            b.id = a.id
    )";
        Assert.Equal(expect, q.CommandText);
    }
}
