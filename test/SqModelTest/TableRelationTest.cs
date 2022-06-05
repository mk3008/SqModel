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
        var srcAlias = new SelectQuery() { Table = src, AliasName= "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new SelectQuery() { Table = dest, AliasName = "d" };

        var tr = new TableRelationClause() { SourceTable = srcAlias, DestinationTable = destAlias };
        tr.AddCondition("column");

        var text = tr.ToQuery().CommandText;
        var expect = @"inner join destination as d on s.column = d.column";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void ColumnName()
    {
        var src = new Table() { TableName = "source" };
        var srcAlias = new SelectQuery() { Table = src, AliasName = "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new SelectQuery() { Table = dest, AliasName = "d" };

        var tr = new TableRelationClause() { SourceTable = srcAlias, DestinationTable = destAlias };
        tr.AddCondition("column_s", "column_d");

        var text = tr.ToQuery().CommandText;
        var expect = @"inner join destination as d on s.column_s = d.column_d";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void LeftJoin()
    {
        var src = new Table() { TableName = "source" };
        var srcAlias = new SelectQuery() { Table = src, AliasName = "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new SelectQuery() { Table = dest, AliasName = "d" };

        var tr = new TableRelationClause() { SourceTable = srcAlias, DestinationTable = destAlias, RelationType =RelationTypes.Left };
        tr.AddCondition("column");

        var text = tr.ToQuery().CommandText;
        var expect = @"left join destination as d on s.column = d.column";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void RightJoin()
    {
        var src = new Table() { TableName = "source" };
        var srcAlias = new SelectQuery() { Table = src, AliasName = "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new SelectQuery() { Table = dest, AliasName = "d" };

        var tr = new TableRelationClause() { SourceTable = srcAlias, DestinationTable = destAlias, RelationType = RelationTypes.Right };
        tr.AddCondition("column");

        var text = tr.ToQuery().CommandText;
        var expect = @"right join destination as d on s.column = d.column";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void CrossJoin()
    {
        var src = new Table() { TableName = "source" };
        var srcAlias = new SelectQuery() { Table = src, AliasName = "s" };
        var dest = new Table() { TableName = "destination" };
        var destAlias = new SelectQuery() { Table = dest, AliasName = "d" };

        var tr = new TableRelationClause() { SourceTable = srcAlias, DestinationTable = destAlias, RelationType = RelationTypes.Cross };

        var text = tr.ToQuery().CommandText;
        var expect = @"cross join destination as d";

        Assert.Equal(expect, text);
    }
}