using SqModel;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class ColumnsTest
{
    [Fact]
    public void Default()
    {
        using var p = new Parser(" t.column_a as c1, t.column_b as c2 ");

        var cols = p.ParseSelectColumns();

        Assert.Equal(2, cols.Count);
        Assert.Equal("t", cols[0].TableName);
        Assert.Equal("column_a", cols[0].ColumnName);
        Assert.Equal("c1", cols[0].AliasName);
        Assert.Equal("t", cols[1].TableName);
        Assert.Equal("column_b", cols[1].ColumnName);
        Assert.Equal("c2", cols[1].AliasName);
    }
}
