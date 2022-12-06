using SqModel.Analysis.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class WhereClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public WhereClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "tbl.col1 = 1 and (tbl.col2 = 2 or tbl.col3 = 3)";
        var item = WhereClauseParser.Parse(text);
        Monitor.Log(item);


    }
}
