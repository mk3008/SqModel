using SqModel;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class TableTest
{
    [Fact]
    public void Default()
    {
        using var p = new Parser(" schema.table_a as t1 ");

        var tbl = p.ParseSelectTable();

        Assert.Equal("schema", tbl.Schema);
        Assert.Equal("table_a", tbl.TableName);
        Assert.Equal("t1", tbl.AliasName);
    }

    [Fact]
    public void OmitSchema()
    {
        using var p = new Parser(" table_a as t1 ");

        var tbl = p.ParseSelectTable();

        Assert.Equal("", tbl.Schema);
        Assert.Equal("table_a", tbl.TableName);
        Assert.Equal("t1", tbl.AliasName);
    }

    [Fact]
    public void Omit()
    {
        using var p = new Parser(" table_a ");

        var tbl = p.ParseSelectTable();

        Assert.Equal("", tbl.Schema);
        Assert.Equal("table_a", tbl.TableName);
        Assert.Equal("", tbl.AliasName);
    }

    [Fact]
    public void OmitAsCommand()
    {
        using var p = new Parser(" table_a t1 ");

        var tbl = p.ParseSelectTable();

        Assert.Equal("", tbl.Schema);
        Assert.Equal("table_a", tbl.TableName);
        Assert.Equal("t1", tbl.AliasName);
    }

    [Fact]
    public void AliasCommandCheck()
    {
        using var p = new Parser(" table_a where ");

        var tbl = p.ParseSelectTable();

        Assert.Equal("", tbl.Schema);
        Assert.Equal("table_a", tbl.TableName);
        Assert.Equal("", tbl.AliasName);
    }
}
