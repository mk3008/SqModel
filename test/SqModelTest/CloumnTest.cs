using SqModel;
using Xunit;

namespace SqModelTest;

public class CloumnTest
{
    [Fact]
    public void Column()
    {
        var c = new Column() { TableName = "a", ColumnName = "column_b" };
        
        var text = c.ToQuery().CommandText;
        var expect = @"a.column_b";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void Alias()
    {
        var c = new Column() { TableName = "a", ColumnName = "column_b", AliasName = "b" };

        var text = c.ToQuery().CommandText;
        var expect = @"a.column_b as b";

        Assert.Equal(expect, text);
    }
}
