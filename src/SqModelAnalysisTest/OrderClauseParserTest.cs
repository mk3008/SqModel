using SqModel.Analysis;
using SqModel.Analysis.Parser;
using SqModel.Core.Clauses;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class OrderClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public OrderClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "tbl.col1, 1, tbl.col2 desc nulls first";
        var item = OrderClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal(3, item.Count);
    }
}