using SqModel;
using SqModel.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class CreateTable
{
    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");

        var tq = new CreateTableQuery() { SelectQuery = q, TableName = "tmp" };

        var text = tq.ToQuery().CommandText;
        var expect = @"create table tmp
as
select table_a.*
from table_a";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Temporary()
    {
        var q = new SelectQuery();
        var table_a = q.From("table_a");
        q.Select(table_a, "*");

        var tq = new CreateTableQuery() { SelectQuery = q, TableName = "tmp", IsTemporary = true };

        var text = tq.ToQuery().CommandText;
        var expect = @"create temporary table tmp
as
select table_a.*
from table_a";

        Assert.Equal(expect, text);
    }
}
