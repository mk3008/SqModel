using SqModel;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class ParseSelectQueryTest
{
    [Fact]
    public void Default()
    {
        using var p = new Parser("select t.column_a as c1 from table as t");

        var q = p.Parse();
        var expect = @"select t.column_a as c1
from table as t";

        Assert.Equal(expect, q.ToQuery().CommandText);

    }

}
