using SqModel.Analysis.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class RelationParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public RelationParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void InnerJoin()
    {
        var text = "inner join public.table as b on a.id = b.id";
        var item = RelationParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal("inner join public.table as b on a.id = b.id", item.GetCommandText());
    }

    [Fact]
    public void LeftJoin()
    {
        var text = "left join public.table as b on a.id = b.id";
        var item = RelationParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal("left join public.table as b on a.id = b.id", item.GetCommandText());
    }

    [Fact]
    public void LeftOuterJoin()
    {
        var text = "left outer join public.table as b on a.id = b.id";
        var item = RelationParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal("left join public.table as b on a.id = b.id", item.GetCommandText());
    }

    [Fact]
    public void RightJoin()
    {
        var text = "right join public.table as b on a.id = b.id";
        var item = RelationParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal("right join public.table as b on a.id = b.id", item.GetCommandText());
    }

    [Fact]
    public void RightOuterJoin()
    {
        var text = "right outer join public.table as b on a.id = b.id";
        var item = RelationParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal("right join public.table as b on a.id = b.id", item.GetCommandText());
    }

    [Fact]
    public void CrossJoin()
    {
        var text = "cross join public.table as b";
        var item = RelationParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal("cross join public.table as b", item.GetCommandText());
    }
}