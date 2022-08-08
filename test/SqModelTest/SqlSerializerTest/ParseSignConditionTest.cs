using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ParseSignConditionTest
{
    public ParseSignConditionTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Number()
    {
        using var p = new SelectQueryParser("col1 = 1");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);
    }

    [Fact]
    public void String()
    {
        using var p = new SelectQueryParser("col1 = 'abc'");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);

    }
    [Fact]
    public void TableColumn()
    {
        using var p = new SelectQueryParser("a.col1 = 1");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);
    }

    [Fact]
    public void JoinCondition()
    {
        using var p = new SelectQueryParser("a.col1 = b.col1");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);
    }

    [Fact]
    public void NotEqual()
    {
        using var p = new SelectQueryParser("a.col1 != b.col1");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);
    }
}
