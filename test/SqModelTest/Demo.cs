using SqModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class Demo
{
    [Fact]
    public void Default()
    {
        var sq = new SelectQuery();
        var ta = sq.From("table_a").As("a");
        var tb = ta.LeftJoin("table_b").As("b").On("id", "table_a_id");

        sq.SelectAll();
        sq.Where.Add().Column(ta, "id").Equal(":id").Parameter(":id", 1);
        sq.Where.Add().Column(tb, "table_a_id").IsNull();
        sq.Where.Add().Column(tb, "is_visible").True();

        var q = sq.ToQuery();

        /*
select *
from table_a as a
left join table_b as b on a.id = b.table_a_id
where
    a.id = :id
    and b.table_a_id is null
    and b.is_visible = true
        */
        Console.WriteLine(q.CommandText);
    }
}
