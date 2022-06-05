using SqModel;
using Xunit;

namespace SqModelTest;

public class SelectSubquery
{
    [Fact]
    public void Default()
    {
        var sub = new SelectQuery();
        var table_a = sub.From("table_a");
        sub.Select(table_a, "*");

        var q = new SelectQuery();
        var a = q.From(sub, "a");
        q.Select(a, "*");

        var text = q.ToQuery().CommandText;
        var expect = @"select a.*
from (
    select table_a.*
    from table_a
) as a";

        Assert.Equal(expect, text);
    }

}
