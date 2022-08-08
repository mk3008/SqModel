using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

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
        using var p = new Parser(sql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseTableClause();

        Assert.Equal("table_a", clause.TableName);
        Assert.Equal("a", clause.AliasName);
        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void JoinTable()
    {
        /// inner join table_b as b on ...
        var sql = "inner join table_b as b on a.column_1 = b.column_1 and a.column_2 and b.column_2";
        using var p = new Parser(sql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseTableClause();

        Assert.Equal("table_b", clause.TableName);
        Assert.Equal("b", clause.AliasName);
        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void Default()
    {
        /// inner join table_b as b on ...
        var sql = @"from table_a as a
inner join table_b as b on a.column_1 = b.column_1 and a.column_2 and b.column_2";
        using var p = new Parser(sql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseTableClause();

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
        using var p = new Parser(sql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseTableClause();

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void SubQuery()
    {
        /// inner join table_b as b on ...
        var sql = @"from (
    select *
    from table_a
) as a";
        using var p = new Parser(sql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseTableClause();

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }

    [Fact]
    public void SubQueryMany()
    {
        /// inner join table_b as b on ...
        var sql = @"from (
    select *
    from table_a
) as a
inner join (
    select *
    from table_b
) as b on a.column_1 = b.column_1";
        using var p = new Parser(sql);
        p.Logger = (x) => Output.WriteLine(x);
        var clause = p.ParseTableClause();

        Assert.Equal(sql, clause.ToQuery().CommandText);
    }
}
