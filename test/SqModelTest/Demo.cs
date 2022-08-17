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
        var ta = sq.From("table_a", "a");
        var tb = ta.LeftJoin("table_b", "b").On("id", "table_a_id");

        sq.SelectAll();
        sq.Where().Value(ta, "id").Equal(":id").AddParameter(":id", 1);
        sq.Where().Value(tb, "table_a_id").IsNull();

        var q = sq.ToQuery();

        /*
select *
from table_a as a
left join table_b as b on a.id = b.table_a_id
where
    a.id = :id --1
    and b.table_a_id is null
        */
        Console.WriteLine(q.CommandText);
    }
}
