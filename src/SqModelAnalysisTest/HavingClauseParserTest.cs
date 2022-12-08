﻿using SqModel.Analysis.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class HavingClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public HavingClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "sum(tbl.col1) = 1 and (sum(tbl.col2) = 2 or sum(tbl.col3) = 3)";
        var item = HavingClauseParser.Parse(text);
        Monitor.Log(item);
    }
}