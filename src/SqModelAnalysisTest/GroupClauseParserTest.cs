﻿using SqModel.Analysis.Parser;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class GroupClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public GroupClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "tbl.col1, 1, tbl.col2";
        var item = GroupClauseParser.Parse(text);
        Monitor.Log(item);
    }
}
