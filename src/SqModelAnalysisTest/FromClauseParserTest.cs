using SqModel.Analysis.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class FromClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public FromClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "public.table_a as a inner join table_b as b on a.id = b.id left join table_c as c on a.id = c.id";
        var item = FromClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal(2, item.Relations?.Count);
    }
}
