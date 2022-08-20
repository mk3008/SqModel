using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;
public class ParseCteTest
{
    public ParseCteTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        var sql = @"with
a as (
    select *
    from table_a
),
b as (
    select *
    from table_b
)
select *
from a
inner join b on a.column_1 = b.column_1";

        using var p = new SqlParser(sql);
        p.Logger = (x) => Output.WriteLine(x);
        var sq = p.ParseSelectQuery();
        var text = sq.ToQuery().CommandText;

        Assert.Equal(sql, text);
    }
}