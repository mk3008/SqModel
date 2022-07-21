using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ParseSelectTableTest
{
    public ParseSelectTableTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Full()
    {
        using var p = new SelectQueryParser("schema_name.table_name as alias_name");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectTable(s);
        Assert.Equal("schema_name", s.Table.Schema);
        Assert.Equal("table_name", s.Table.TableName);
        Assert.Equal("alias_name", s.Table.AliasName);
    }

    [Fact]
    public void OmitSchema()
    {
        using var p = new SelectQueryParser("table_name as alias_name");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectTable(s);
        Assert.Equal("", s.Table.Schema);
        Assert.Equal("table_name", s.Table.TableName);
        Assert.Equal("alias_name", s.Table.AliasName);
    }

    [Fact]
    public void OmitAliasToken()
    {
        using var p = new SelectQueryParser("table_name alias_name");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectTable(s);
        Assert.Equal("", s.Table.Schema);
        Assert.Equal("table_name", s.Table.TableName);
        Assert.Equal("alias_name", s.Table.AliasName);
    }

    [Fact]
    public void Brief()
    {
        using var p = new SelectQueryParser("table_name");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectTable(s);
        Assert.Equal("", s.Table.Schema);
        Assert.Equal("table_name", s.Table.TableName);
        Assert.Equal("table_name", s.Table.AliasName);
    }
}
