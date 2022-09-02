using SqModel.Analysis;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ReadTokensTest
{
    public ReadTokensTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Split()
    {
        var text = "a b\rc\nd\r\ne\tf";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(6, lst.Count);
        Assert.Equal("a", lst[0]);
        Assert.Equal("b", lst[1]);
        Assert.Equal("c", lst[2]);
        Assert.Equal("d", lst[3]);
        Assert.Equal("e", lst[4]);
        Assert.Equal("f", lst[5]);
    }

    [Fact]
    public void Word()
    {
        var text = "a bc def";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("a", lst[0]);
        Assert.Equal("bc", lst[1]);
        Assert.Equal("def", lst[2]);
    }

    [Fact]
    public void ReservedWord()
    {
        var text = "left outer join a";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(2, lst.Count);
        Assert.Equal("left outer join", lst[0]);
        Assert.Equal("a", lst[1]);
    }

    [Fact]
    public void Symbol()
    {
        var text = ". .. ...";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal(".", lst[0]);
        Assert.Equal("..", lst[1]);
        Assert.Equal("...", lst[2]);
    }

    [Fact]
    public void WordAndSymbol()
    {
        var text = "a.b";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("a", lst[0]);
        Assert.Equal(".", lst[1]);
        Assert.Equal("b", lst[2]);
    }

    [Fact]
    public void Quote()
    {
        var text = "start'comment text'end";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("'comment text'", lst[1]);
        Assert.Equal("end", lst[2]);
    }

    [Fact]
    public void QuoteEscape()
    {
        var text = "start'comment''text'end";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("'comment''text'", lst[1]);
        Assert.Equal("end", lst[2]);
    }

    [Fact]
    public void Bracket()
    {
        var text = "start(comment text)end";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(5, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("(", lst[1]);
        Assert.Equal("comment text", lst[2]);
        Assert.Equal(")", lst[3]);
        Assert.Equal("end", lst[4]);
    }

    [Fact]
    public void BracketNest()
    {
        var text = "start((comment)(text))end";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(5, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("(", lst[1]);
        Assert.Equal("(comment)(text)", lst[2]);
        Assert.Equal(")", lst[3]);
        Assert.Equal("end", lst[4]);
    }

    [Fact]
    public void LineComment()
    {
        var text = "start-- comment text\r\nend";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("-- comment text", lst[1]);
        Assert.Equal("end", lst[2]);
    }

    [Fact]
    public void BlockComment()
    {
        var text = "start/* comment\r\ntext */end";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("/* comment\r\ntext */", lst[1]);
        Assert.Equal("end", lst[2]);
    }

    [Fact]
    public void BlockCommentNest()
    {
        var text = "start/* comment /* nest */ */end";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("/* comment /* nest */ */", lst[1]);
        Assert.Equal("end", lst[2]);
    }

    [Fact]
    public void LineCommentInBlockComment()
    {
        var text = "start--/* comment */blockend\r\nlineend";
        using var p = new SqlParser(text);
        p.Logger = (x) => Output.WriteLine(x);
        var lst = p.ReadTokens().ToList();

        Assert.Equal(3, lst.Count);
        Assert.Equal("start", lst[0]);
        Assert.Equal("--/* comment */blockend", lst[1]);
        Assert.Equal("lineend", lst[2]);
    }
}
