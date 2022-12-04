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

public class SelectClauseParseTest
{
    private readonly QueryCommandMonitor Monitor;

    public SelectClauseParseTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "tbl.col1 as col1, tbl.col1 as c1, tbl.col2 c2, tbl.col3, 3.14 as val, 1.23, (tbl.col1 + tbl.col2) / tbl.col3 as colcalc, (1+2)*3 as numcalc";
        var item = SelectClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal(8, item.Count);
    }
}