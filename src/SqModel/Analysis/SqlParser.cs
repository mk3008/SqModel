using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public SqlParser(string text)
    {
        Text = text;
        Reader = new StringReader(Text);
    }

    public static SelectQuery Parse(string text)
    {
        using var p = new SqlParser(text);
        return p.ParseSelectQuery();
    }

    public string Text { get; init; }

    public StringReader Reader { get; private set; }

    public Action<string>? Logger { get; set; }

    public static char[] SpaceTokens = " \t\r\n;".ToCharArray();

    public static char[] SymbolTokens = "+-*/.,()!=%<>'".ToCharArray();

    public static char[] LetterChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

    public static char[] ArithmeticOperatorTokens = "+-*/%".ToCharArray();

    public static string[] LogicalOperatorTokens = new[] {
        "and",
        "or"
    };

    public static string[] FromTokens = new[]
    {
        "from",
    };

    public static string[] InnerJoinTokens = new[]
    {
        "inner join",
    };

    public static string[] LeftJoinTokens = new[]
    {
        "left join",
        "left outer join",
    };

    public static string[] RightJoinTokens = new[]
    {
        "right join",
        "right outer join",
    };

    public static string[] CrossJoinTokens = new[]
    {
        "cross join",
    };

    public static string[] WhereTokens = new[]
    {
        "where",
    };

    public static string[] GroupTokens = new[]
    {
        "group by",
    };

    public static string[] HavingTokens = new[]
    {
        "having",
    };

    public static string[] UnionTokens = new[]
    {
        "union",
    };

    public static string[] OrderTokens = new[]
    {
        "order by",
    };

    public static string[] QueryBreakTokens = new[]
    {
        ";",
    };

    public static List<string> ColumnSplitTokens = new() {
        ",",
    };

    private static string[] SignTokens = new[] {
        "=",
        "!=",
        ">",
        ">=",
        "<",
        "<=",
        "is",
    };

    private static string[] AliasTokens = new[] {
        "as",
    };

    internal string[] AliasBreakTokens =
        ColumnSplitTokens
        .Union(FromTokens)
        .Union(InnerJoinTokens)
        .Union(LeftJoinTokens)
        .Union(RightJoinTokens)
        .Union(CrossJoinTokens)
        .Union(WhereTokens)
        .Union(GroupTokens)
        .Union(HavingTokens)
        .Union(UnionTokens)
        .Union(OrderTokens)
        .Union(QueryBreakTokens).ToArray();

    internal string[] TableBreakTokens =
        ColumnSplitTokens
        .Union(InnerJoinTokens)
        .Union(LeftJoinTokens)
        .Union(RightJoinTokens)
        .Union(CrossJoinTokens)
        .Union(WhereTokens)
        .Union(GroupTokens)
        .Union(HavingTokens)
        .Union(UnionTokens)
        .Union(OrderTokens)
        .Union(QueryBreakTokens).ToArray();

    internal string[] ValueBreakTokens =
        ColumnSplitTokens
        .Union(FromTokens)
        .Union(InnerJoinTokens)
        .Union(LeftJoinTokens)
        .Union(RightJoinTokens)
        .Union(CrossJoinTokens)
        .Union(WhereTokens)
        .Union(GroupTokens)
        .Union(HavingTokens)
        .Union(UnionTokens)
        .Union(OrderTokens)
        .Union(QueryBreakTokens)
        .Union(SignTokens)
        .Union(LogicalOperatorTokens)
        .Union(AliasTokens).ToArray();

    public string[] ConditionBreakTokens =
        InnerJoinTokens
        .Union(LeftJoinTokens)
        .Union(RightJoinTokens)
        .Union(CrossJoinTokens)
        .Union(WhereTokens)
        .Union(GroupTokens)
        .Union(HavingTokens)
        .Union(UnionTokens)
        .Union(OrderTokens)
        .Union(QueryBreakTokens).ToArray();

    public char Read()
    {
        var i = Reader.Read();

        if (i.IsEof()) throw new EndOfStreamException();

        return (char)i;
    }

    public char Peek()
    {
        var i = Reader.Peek();
        if (i.IsEof()) throw new EndOfStreamException();
        return (char)i;
    }

    public char? PeekOrDefault()
    {
        var i = Reader.Peek();
        if (i.IsEof()) return null;
        return (char)i;
    }
}
