using SqModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class ColumnRelationSetTest
{
    [Fact]
    public void Default()
    {
        var left = new RelatedColumn() {TableName = "left", ColumnName = "column" };
        var right = new RelatedColumn() { TableName = "right", ColumnName = "column" };

        var rel = new ColumnRelation() { SourceColumn = left, DestinationColumn = right };

        var text = rel.ToQuery().CommandText;
        var expect = @"left.column = right.column";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Sign()
    {
        var left = new RelatedColumn() { TableName = "left", ColumnName = "column" };
        var right = new RelatedColumn() { TableName = "right", ColumnName = "column" };

        var rel = new ColumnRelation() { SourceColumn = left, DestinationColumn = right , Sign = "<>"};

        var text = rel.ToQuery().CommandText;
        var expect = @"left.column <> right.column";

        Assert.Equal(expect, text);
    }
}