using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ParseClosureConditionTest
{
    public ParseClosureConditionTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void AndConjunction()
    {
        using var p = new SelectQueryParser("a.col1 = b.col1 and a.col2 = b.col2");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);
    }

    [Fact]
    public void OrConjunction()
    {
        using var p = new SelectQueryParser("a.col1 = b.col1 or a.col2 = b.col2");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);
    }

    [Fact]
    public void Closure()
    {
        using var p = new SelectQueryParser("(a.col1 = b.col1 or a.col2 = b.col2) and (a.col3 = b.col3)");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);
    }

    [Fact]
    public void ClosureNest()
    {
        using var p = new SelectQueryParser("(a.col3 = b.col3 or (a.col1 = b.col1 or a.col2 = b.col2))");
        p.Logger = (x) => Output.WriteLine(x);

        var s = new SelectTokenSet();
        var r = p.ParseSelectColumn(s);
    }

}
