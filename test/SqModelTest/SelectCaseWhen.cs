using SqModel;
using SqModel.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class SelectCaseWhen
{
    [Fact]
    public void DefaultCaseWhen()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a").As("a");

        q.Select.Add().CaseWhen(x =>
        {
            x.Add().When(w => w.Value("a").Equal(1)).Then(10);
            x.Add().When(w => w.Column("a", "id").Equal(2)).Then(20);
            x.Add().When(w => w.Column(ta, "id").Equal(3)).Then(30);
            x.Add().WhenGroup(g =>
            {
                g.Add().Column("a", "id").Equal(1);
                g.Add().Or().Column("b", "id").Equal(2);
            }).Then(40);
        }).As("case_1");

        var text = q.ToQuery().CommandText;
        var expect = @"select case when a = 1 then 10 when a.id = 2 then 20 when a.id = 3 then 30 when (a.id = 1 or b.id = 2) then 40 end as case_1
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void DefaultCase()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a").As("a");

        q.Select.Add().Case("1", x =>
        {
            x.Add().When("a").Then(10);
            x.Add().When("a", "id").Then(20);
            x.Add().When(ta, "id").Then(30);
            x.Add().When(1).Then(30);
            x.Add().When(1).ThenNull();
            x.Add().Else(100);
        }).As("case_2");

        var text = q.ToQuery().CommandText;
        var expect = @"select case 1 when a then 10 when a.id then 20 when a.id then 30 when 1 then 30 when 1 then null else 100 end as case_2
from table_a as a";

        Assert.Equal(expect, text);
    }
}
