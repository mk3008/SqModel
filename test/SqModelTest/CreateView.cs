using SqModel;
using System.Linq;
using Xunit;

namespace SqModelTest;

public class CreateView
{
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");

        var tq = new CreateViewQuery() { SelectQuery = q, ViewName = "tmp" };

        var text = tq.ToQuery().CommandText;
        var expect = @"create view tmp
as
select
    table_a.*
from table_a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Temporary()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");

        var tq = new CreateViewQuery() { SelectQuery = q, ViewName = "tmp", IsTemporary = true };

        var text = tq.ToQuery().CommandText;
        var expect = @"create temporary view tmp
as
select
    table_a.*
from table_a";

        Assert.Equal(expect, text);
    }
}
