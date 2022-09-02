using SqModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class Insert
{
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(table_a, "id");

        var tq = new InsertQuery() { SelectQuery = q, TableName = "table_b" };

        var text = tq.ToQuery().CommandText;
        var expect = @"insert into table_b(id)
select a.id
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void ColumnNameOmitted()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(table_a, "*");

        var tq = new InsertQuery() { SelectQuery = q, TableName = "table_b" };

        var text = tq.ToQuery().CommandText;
        var expect = @"insert into table_b
select a.*
from table_a as a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void ColumnNameAlias()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a", "a");
        q.Select(table_a, "id").As("index_value");

        var tq = new InsertQuery() { SelectQuery = q, TableName = "table_b" };

        var text = tq.ToQuery().CommandText;
        var expect = @"insert into table_b(index_value)
select a.id as index_value
from table_a as a";

        Assert.Equal(expect, text);
    }
}
