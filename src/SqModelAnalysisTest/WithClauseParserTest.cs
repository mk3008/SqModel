﻿using SqModel.Analysis;
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

public class WithClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public WithClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "cte1 as (select id from table_a), cte2 as (select id from table_b)";
        var item = WithClauseParser.Parse(text);
        Monitor.Log(item);
    }

    [Fact]
    public void Recursive()
    {
        var text = "recursive cte1 as (select id from table_a), cte2 as (select id from table_b)";
        var item = WithClauseParser.Parse(text);
        Monitor.Log(item);
    }

    [Fact]
    public void Materialized()
    {
        var text = "cte1 as materialized (select id from table_a), cte2 as not materialized (select id from table_b)";
        var item = WithClauseParser.Parse(text);
        Monitor.Log(item);
    }
}