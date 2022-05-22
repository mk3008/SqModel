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
        var left = new ColumnRelation() {TableName = "left", ColumnName = "column" };
        var right = new ColumnRelation() { TableName = "right", ColumnName = "column" };

        var rel = new ColumnRelationSet() { LeftColumn = left, RightColumn = right };

        var text = rel.ToQuery().CommandText;
        var expect = @"left.column = right.column";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Sign()
    {
        var left = new ColumnRelation() { TableName = "left", ColumnName = "column" };
        var right = new ColumnRelation() { TableName = "right", ColumnName = "column" };

        var rel = new ColumnRelationSet() { LeftColumn = left, RightColumn = right , Sign = "<>"};

        var text = rel.ToQuery().CommandText;
        var expect = @"left.column <> right.column";

        Assert.Equal(expect, text);
    }
}