﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public Parser(string text)
    {
        Text = text;
        Reader = new StringReader(Text);
    }

    public string Text { get; init; }

    public StringReader Reader { get; private set; }

    public Action<string>? Logger { get; set; }

    public static char[] SpaceTokens = " \t\r\n;".ToCharArray();

    public static char[] SymbolTokens = "+-*/.,()!=%<>'".ToCharArray();

    public static char[] LetterChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

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

    public static string[] QueryBreakTokens = new[]
    {
        ";",
    };

    private static string[] ColumnSplitTokens = new[] {
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

    private string[] ValueBreakTokens =
        ColumnSplitTokens
        .Union(FromTokens)
        .Union(InnerJoinTokens)
        .Union(LeftJoinTokens)
        .Union(RightJoinTokens)
        .Union(CrossJoinTokens)
        .Union(WhereTokens)
        .Union(QueryBreakTokens)
        .Union(SignTokens)
        .Union(LogicalOperatorTokens).ToArray();

    private string[] ConditionBreakTokens =
        InnerJoinTokens
        .Union(LeftJoinTokens)
        .Union(RightJoinTokens)
        .Union(CrossJoinTokens)
        .Union(WhereTokens)
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
