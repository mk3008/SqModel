using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class TransactionTest
{
    [Fact]
    public void Default()
    {
        using var p = new Parser("abc");

        var c = p.Read();
        Assert.Equal('a', c);

        c = p.Read();
        Assert.Equal('b', c);

        c = p.Read();
        Assert.Equal('c', c);
    }

    [Fact]
    public void Commit()
    {
        using var p = new Parser("abc");

        var c = p.Read();
        Assert.Equal('a', c);
        
        p.BeginTransaction();
        c = p.Read();
        Assert.Equal('b', c);
        p.Commit();

        c = p.Read();
        Assert.Equal('c', c);
    }

    [Fact]
    public void Rollback()
    {
        using var p = new Parser("abc");

        var c = p.Read();
        Assert.Equal('a', c);

        p.BeginTransaction();
        c = p.Read();
        Assert.Equal('b', c);
        p.RollBack();

        c = p.Read();
        Assert.NotEqual('c', c);
        Assert.Equal('b', c);

        c = p.Read();
        Assert.Equal('c', c);
    }
}
