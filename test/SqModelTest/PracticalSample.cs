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
    [Fact]
    public void SelectManyTable()
    {
        var sd = (new Table("sales_detail", "sd")).ToTableAlias();
        var s = (new Table("sales", "s")).ToTableAlias();
        var a = (new Table("article", "a")).ToTableAlias();

        var q = new SelectQuery();

        q.Root = sd;
        q.AddTableRelation(sd, s).AddColumnRelation("sales_id");
        q.AddTableRelation(sd, a).AddColumnRelation("article_id");

        q.AddColumn(s, "sales_id");
        q.AddColumn(s, "sales_date");
        q.AddColumn(sd, "sales_detail_id");
        q.AddColumn(a, "article_id");
        q.AddColumn(a, "article_name");
        q.AddColumn(sd, "amount");

        var text = q.ToQuery().CommandText;
        var expect = @"select s.sales_id, s.sales_date, sd.sales_detail_id, a.article_id, a.article_name, sd.amount
from sales_detail as sd
inner join sales as s on sd.sales_id = s.sales_id
inner join article as a on sd.article_id = a.article_id";

        Assert.Equal(expect, text);
    }
}
