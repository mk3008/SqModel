using SqModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class TableRelationTest
{
    [Fact]
    public void Default()
    {
        var src = new Table() { TableName = "source" };
        var srcAlias = new TableAlias() { Table = src, AliasName= "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new TableAlias() { Table = dest, AliasName = "d" };

        var tr = new TableRelation() { SourceTable = srcAlias, DestinationTable = destAlias };
        tr.AddColumnRelation("column");

        var text = tr.ToQuery().CommandText;
        var expect = @"inner join destination as d on s.column = d.column";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void ColumnName()
    {
        var src = new Table() { TableName = "source" };
        var srcAlias = new TableAlias() { Table = src, AliasName = "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new TableAlias() { Table = dest, AliasName = "d" };

        var tr = new TableRelation() { SourceTable = srcAlias, DestinationTable = destAlias };
        tr.AddColumnRelation("column_s", "column_d");

        var text = tr.ToQuery().CommandText;
        var expect = @"inner join destination as d on s.column_s = d.column_d";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void LeftJoin()
    {
        var src = new Table() { TableName = "source" };
        var srcAlias = new TableAlias() { Table = src, AliasName = "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new TableAlias() { Table = dest, AliasName = "d" };

        var tr = new TableRelation() { SourceTable = srcAlias, DestinationTable = destAlias, RelationType =RelationTypes.Left };
        tr.AddColumnRelation("column");

        var text = tr.ToQuery().CommandText;
        var expect = @"left join destination as d on s.column = d.column";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void RightJoin()
    {
        var src = new Table() { TableName = "source" };
        var srcAlias = new TableAlias() { Table = src, AliasName = "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new TableAlias() { Table = dest, AliasName = "d" };

        var tr = new TableRelation() { SourceTable = srcAlias, DestinationTable = destAlias, RelationType = RelationTypes.Right };
        tr.AddColumnRelation("column");

        var text = tr.ToQuery().CommandText;
        var expect = @"right join destination as d on s.column = d.column";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void CrossJoin()
    {
        var src = new Table() { TableName = "source" };
        var srcAlias = new TableAlias() { Table = src, AliasName = "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new TableAlias() { Table = dest, AliasName = "d" };

        var tr = new TableRelation() { SourceTable = srcAlias, DestinationTable = destAlias, RelationType = RelationTypes.Cross };

        var text = tr.ToQuery().CommandText;
        var expect = @"cross join destination as d";

        Assert.Equal(expect, text);
    }
}