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

        var left = new ColumnRelation() { TableName = "source", ColumnName = "column" };
        var right = new ColumnRelation() { TableName = "destination", ColumnName = "column" };
        var rel = new ColumnRelationSet() { LeftColumn = left, RightColumn = right };

        var tr = new TableRelation() { SourceTable = srcAlias, DestinationTable = destAlias };
        tr.ColumnRelationSets.Add(rel);

        var text = tr.ToQuery().CommandText;
        var expect = @"inner join destination as d on s.column = d.column";

        Assert.Equal(expect, text);
    }
}