using System;
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

    //private string[] CommandTokens = new[]
    //{
    //    "with" ,
    //    "select",
    //    "distinct",
    //    "limit",
    //    "as",
    //    "from",
    //    "inner",
    //    "left",
    //    "right",
    //    "cross",
    //    "where",
    //    "group",
    //    "having",
    //    "order",
    //    "and",
    //    "or",
    //};

    //private static string LineCommentToken = "--";

    //private static string BlockCommentToken = "/*";

    public static char[] SpaceTokens = " \t\r\n;".ToCharArray();

    public static char[] SymbolTokens = "+-*/.,()".ToCharArray();

    public static char[] LetterChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

    //private int Index { get; set; }

    //private bool IsTransaction { get; set; } = false;

    //private int TransactionIndex { get; set; } = 0;

    //public void BeginTransaction()
    //{
    //    if (IsTransaction) throw new InvalidOperationException();
    //    IsTransaction = true;
    //}

    //public void Commit()
    //{
    //    if (!IsTransaction) throw new InvalidOperationException();
    //    IsTransaction = false;
    //    Index = TransactionIndex;
    //}

    //public void RollBack()
    //{
    //    if (!IsTransaction) throw new InvalidOperationException();
    //    IsTransaction = false;
    //    Reader.Dispose();
    //    Reader = new StringReader(Text);
    //    for (int i = 0; i < Index; i++) Reader.Read();
    //}

    //public char? PrevChar { get; private set; } = null;

    //public char? CurrentChar { get; private set; } = null;

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

    //public string ReadUntilSpace()
    //{
    //    var digit = (char c) => !c.IsSpace();
    //    return ReadUntil(digit);
    //}

    //public char? ReadChar(string chars)
    //{
    //    var digit = (char c) => chars.Select(x => c == x.ToLower() || c == x.ToUpper()).Any();
    //    return ReadChar(digit);
    ////}

    //public char? ReadChar(Func<char, bool> digit)
    //{
    //    var i = Reader.Peek();
    //    if (i.IsNotEof() && digit(i.ToChar()))
    //    {
    //        Read();
    //        return (char)i;
    //    }
    //    return null;
    //}

    //public string ReadUntil(string splitters, char keychar) => ReadUntil($"{splitters}{keychar}");

    //public string ReadUntil(string splitters)
    //{
    //    var sb = new StringBuilder();
    //    splitters.ToList().ForEach(x =>
    //    {
    //        var lower = x.ToLower();
    //        var upper = x.ToUpper();
    //        sb.Append(lower);
    //        if (lower != upper) sb.Append(upper);
    //    });
    //    var s = sb.ToString();
    //    var digit = (char c) =>
    //    {
    //        return s.IndexOf(c) == -1;
    //    };
    //    return ReadUntil(digit);
    //}

    //public string ReadUntil(Func<char, bool> digit)
    //{
    //    var sb = new StringBuilder();
    //    var fn = () =>
    //    {
    //        var i = Reader.Peek();
    //        if (i.IsNotEof() && digit(i.ToChar()))
    //        {
    //            sb.Append(i.ToChar());
    //            Read();
    //            return true;
    //        }
    //        return false;
    //    };
    //    while (fn()) { }
    //    return sb.ToString();
    //}

    //public bool IsSpace(char c) => " \r\n\t".IndexOf(c) >= 0;

    //public int ReadSkipSpaces()
    //{
    //    return ReadSkipWhile(x => x.IsSpace());
    //}

    //public int ReadSkipWhile(Func<char, bool> digit)
    //{
    //    var i = Reader.Peek();
    //    if (i.IsNotEof() && digit(i.ToChar()))
    //    {
    //        Read();
    //        return ReadSkipWhile(digit);
    //    }
    //    return i;
    //}

    //public SelectQuery? Parse()
    //{
    //    ReadSkipSpaces();

    //    var res = ReadUntilCommand();

    //    if (res.IsSuccess == false) throw new Exception();

    //    return null;
    //}
}
