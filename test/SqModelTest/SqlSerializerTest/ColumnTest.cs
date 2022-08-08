using SqModel;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class ColumnTest
{
    [Fact]
    public void Default()
    {
        using var p = new Parser(" t.column as c1 ");

        var c = p.ParseSelectColumn();

        Assert.Equal("t", c.TableName);
        Assert.Equal("column", c.ColumnName);
        Assert.Equal("c1", c.AliasName);
    }

    [Fact]
    public void OmitTable()
    {
        using var p = new Parser(" column as c1 ");

        var c = p.ParseSelectColumn();

        Assert.Equal("", c.TableName);
        Assert.Equal("column", c.ColumnName);
        Assert.Equal("c1", c.AliasName);
    }

    [Fact]
    public void OmitAlias()
    {
        using var p = new Parser(" t.column ");

        var c = p.ParseSelectColumn();

        Assert.Equal("t", c.TableName);
        Assert.Equal("column", c.ColumnName);
        Assert.Equal("", c.AliasName);
    }

    [Fact]
    public void OmitAll()
    {
        using var p = new Parser(" column ");

        var c = p.ParseSelectColumn();

        Assert.Equal("", c.TableName);
        Assert.Equal("column", c.ColumnName);
        Assert.Equal("", c.AliasName);
    }
}
