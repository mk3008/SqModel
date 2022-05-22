using SqModel;
using Xunit;

namespace SqModelTest;

public class VirtualColumnTest
{
    [Fact]
    public void StaticColumn()
    {
        var c = new VirtualColumn() { CommandText = "1", AliasName = "val_b"};
        
        var text = c.ToQuery().CommandText;
        var expect = @"1 as val_b";

        Assert.Equal(expect, text);
    }

    [Fact]
    public void VariableColumn()
    {
        var c = new VirtualColumn() { CommandText = ":val", AliasName = "val_b" };
        c.AddParameter(":val", 1);

        var q = c.ToQuery();
        var text = q.CommandText;
        var expect = @":val as val_b";

        Assert.Equal(expect, text);

        Assert.Single(q.Parameters);
        Assert.Equal(1, q.Parameters[":val"]);
    }
}
