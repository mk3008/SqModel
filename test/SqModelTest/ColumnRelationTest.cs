using SqModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest;

public class ColumnRelationTest
{
    [Fact]
    public void Default()
    {
        var cr = new RelatedColumn() {TableName = "a", ColumnName = "b" };

        var text = cr.ToQuery().CommandText;
        var expect = @"a.b";

        Assert.Equal(expect, text);
    }
}