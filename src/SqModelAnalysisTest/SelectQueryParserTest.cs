using SqModel.Analysis;
using SqModel.Analysis.Parser;
using SqModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class SelectQueryParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public SelectQueryParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void SortSample()
    {
        var text = @"
select
    *
from 
    table_a a
order by 
    a.name nulls first,
    a.val desc,
    a.table_a_id";

        var item = SelectQueryParser.Parse(text);
        Monitor.Log(item);

        Assert.NotNull(item.SelectClause);
        Assert.Single(item.SelectClause);
        Assert.NotNull(item.OrderClause);
        Assert.Equal(3, item.OrderClause!.Count());
    }

    [Fact]
    public void RelationSample()
    {
        var text = @"
select
    a.table_a_id as id,
    3.14 as val,
    (a.val + b.val) * 2 as calc, 
    b.table_b_id,
    c.table_c_id
from 
    table_a a
    inner join table_b b on a.table_a_id = b.table_a_id and b.visible = true
    left join table_c c on a.table_a_id = c.table_a_id";

        var item = SelectQueryParser.Parse(text);
        Monitor.Log(item);

        Assert.NotNull(item.SelectClause);
        Assert.Equal(5, item.SelectClause!.Count);
        Assert.Equal(2, item.FromClause!.Relations!.Count());
    }

    [Fact]
    public void GroupSample()
    {
        var text = @"
select
    a.name,
    a.sub_name,
    sum(a.val) as val
from 
    table_a a
group by
    a.name,
    a.sub_name
having
    sum(a.val) > 0";

        var item = SelectQueryParser.Parse(text);
        Monitor.Log(item);

        Assert.NotNull(item.SelectClause);
        Assert.Equal(3, item.SelectClause!.Count);
        Assert.NotNull(item.GroupClause);
        Assert.Equal(2, item.GroupClause!.Count());
        Assert.NotNull(item.HavingClause);
    }

    [Fact]
    public void Union()
    {
        var text = @"
select
    a.id
from
    table_a as a
union
select
    b.id
from
    table_b as b
union all
select
    c.id
from
    table_c as c";

        var item = SelectQueryParser.Parse(text);
        Monitor.Log(item);
    }
}