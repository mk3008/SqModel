using SqModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class PracticalSample
{
    private SelectQuery CreateSelectQuery()
    {
        var q = new SelectQuery();

        var sd = q.From("sales_detail", "sd");
        var s = sd.InnerJoin("sales", "s", new() { "sales_id" });
        var a = sd.InnerJoin("article", "a", new() { "article_id" });

        q.Select(s, "sales_id");
        q.Select(s, "sales_date");
        q.Select(sd, "sales_detail_id");
        q.Select(a, "article_id");
        q.Select(a, "article_name");
        q.Select(sd, "amount");
        q.Select("current_timestamp", "select_timestamp");
        q.Select(":v1 + :v2", "value").AddParameter(":v1", 1).AddParameter(":v2", 2);

        return q;
    }

    [Fact]
    public void SelectManyTable()
    {
        var q = CreateSelectQuery();

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"select s.sales_id, s.sales_date, sd.sales_detail_id, a.article_id, a.article_name, sd.amount, current_timestamp as select_timestamp, :v1 + :v2 as value
from sales_detail as sd
inner join sales as s on sd.sales_id = s.sales_id
inner join article as a on sd.article_id = a.article_id";

        Assert.Equal(expect, text);

        Assert.Equal(1, actual.Parameters[":v1"]);
        Assert.Equal(2, actual.Parameters[":v2"]);
    }

    [Fact]
    public void SelectManyTable2()
    {
        var q = new SelectQuery();

        q.With.Add(CreateSelectQuery(), "x");
        var a = q.From("x");
        q.Select(a, "*");

        var actual = q.ToQuery();
        var text = actual.CommandText;
        var expect = @"with
x as (
    select s.sales_id, s.sales_date, sd.sales_detail_id, a.article_id, a.article_name, sd.amount, current_timestamp as select_timestamp, :v1 + :v2 as value
    from sales_detail as sd
    inner join sales as s on sd.sales_id = s.sales_id
    inner join article as a on sd.article_id = a.article_id
)
select x.*
from x";

        Assert.Equal(expect, text);
    }
}
