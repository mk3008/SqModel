using SqModel;
using SqModel.Analysis;
using SqModel.Extension;
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
    :modelid as ModelId
    , :modelname as ModelName
    , :price as Price";

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
    :modelid as ModelId
    , :modelname as ModelName
    , :price as Price";

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
        keys.Add("ModelId");
        var q = sq.ToInsertQuery("models", keys);

        var sql = @"insert into models(ModelName, Price)
select
    d.ModelName
    , d.Price
from (
    select
        :modelname as ModelName
        , :price as Price
) as d";

        Assert.Equal(sql, q.CommandText);
        Assert.Equal("test", q.Parameters[":modelname"]);
        Assert.Equal(10, q.Parameters[":price"]);
    }

    [Fact]
    public void ObjectToInsertQueryTest2()
    {
        var c = new Model() { ModelName = "test", Price = 10 };

        var sq = SqlParser.Parse(c, nameconverter: x => x.ToSnakeCase().ToLower());

        //remove seq column
        var keys = new List<string>();
        keys.Add("model_id");
        var q = sq.ToInsertQuery("models", keys);

        var sql = @"insert into models(model_name, price)
select
    d.model_name
    , d.price
from (
    select
        :modelname as model_name
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
        keys.Add("ModelId");
        var q = sq.ToUpdateQuery("models", keys);

        var sql = @"update models as t
set
    ModelName = d.ModelName
    , Price = d.Price
from (
    select
        :modelid as ModelId
        , :modelname as ModelName
        , :price as Price
) as d
where
    t.ModelId = d.ModelId";

        Assert.Equal(sql, q.CommandText);
        Assert.Equal(0, q.Parameters[":modelid"]);
        Assert.Equal("test", q.Parameters[":modelname"]);
        Assert.Equal(10, q.Parameters[":price"]);
    }

    [Fact]
    public void ObjectSelectQueryTest()
    {
        var sq = SqlParser.Parse<Model>();

        var q = sq.ToQuery();
        var sql = @"select
    t.modelid
    , t.modelname
    , t.price
from model as t";

        Assert.Equal(sql, q.CommandText);
    }
}
