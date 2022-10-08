using SqModel;
using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class ORMTest
{
    public class Model
    {
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public int? Price { get; set; }
    }

    [Fact]
    public void ObjectToSelectQueryTest()
    {
        var c = new Model() { ModelName = "test", Price = 10 };

        var sq = SqlParser.Parse(c);
        var q = sq.ToQuery();

        var sql = @"select
    :modelid as modelid
    , :modelname as modelname
    , :price as price";

        Assert.Equal(sql, q.CommandText);
        Assert.Equal(0, q.Parameters[":modelid"]);
        Assert.Equal("test", q.Parameters[":modelname"]);
        Assert.Equal(10, q.Parameters[":price"]);
    }

    [Fact]
    public void NullValueTest()
    {
        var c = new Model() { ModelName = "test", Price = null };

        var sq = SqlParser.Parse(c);
        var q = sq.ToQuery();

        var sql = @"select
    :modelid as modelid
    , :modelname as modelname
    , :price as price";

        Assert.Equal(sql, q.CommandText);
        Assert.Equal(0, q.Parameters[":modelid"]);
        Assert.Equal("test", q.Parameters[":modelname"]);
        Assert.Null(q.Parameters[":price"]);
    }

    [Fact]
    public void ObjectToInsertQueryTest()
    {
        var c = new Model() { ModelName = "test", Price = 10 };

        var sq = SqlParser.Parse(c);

        //remove seq column
        var keys = new List<string>();
        keys.Add("modelid");
        var q = sq.ToInsertQuery("models", keys);

        var sql = @"insert into models(modelname, price)
select
    d.modelname
    , d.price
from (
    select
        :modelid as modelid
        , :modelname as modelname
        , :price as price
) as d";

        Assert.Equal(sql, q.CommandText);
        Assert.Equal("test", q.Parameters[":modelname"]);
        Assert.Equal(10, q.Parameters[":price"]);
    }

    [Fact]
    public void ObjectToUpdateQueryTest()
    {
        var c = new Model() { ModelName = "test", Price = 10 };

        var sq = SqlParser.Parse(c);

        var keys = new List<string>();
        keys.Add("modelid");
        var q = sq.ToUpdateQuery("models", keys);

        var sql = @"update models as t
set
    modelname = d.modelname
    , price = d.price
from (
    select
        :modelid as modelid
        , :modelname as modelname
        , :price as price
) as d
where
    t.modelid = d.modelid";

        Assert.Equal(sql, q.CommandText);
        Assert.Equal(0, q.Parameters[":modelid"]);
        Assert.Equal("test", q.Parameters[":modelname"]);
        Assert.Equal(10, q.Parameters[":price"]);
    }
}
