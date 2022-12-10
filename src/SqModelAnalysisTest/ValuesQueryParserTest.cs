﻿using SqModel.Analysis;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class ValuesQueryParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public ValuesQueryParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = @"
values
    (1,1),
    (2,2)
order by 1 desc 
limit 1";

        var item = ValuesQueryParser.Parse(text);
        Monitor.Log(item);
    }

}