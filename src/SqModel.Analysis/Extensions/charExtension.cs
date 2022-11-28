using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis.Extensions;

public static class charExtension
{
    //public static bool IsSpace(this char? source)
    //{
    //    if (source == null) return false;
    //    return source.Value.IsSpace();
    //}

    //public static bool IsSpace(this char source)
    //{
    //    return " \r\n\t".Contains(source);
    //}

    //public static bool IsLetter(this char? source)
    //{
    //    if (source == null) return false;
    //    return source.Value.IsLetter();
    //}

    //public static bool IsLetter(this char source)
    //{
    //    return "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(source);
    //}

    public static bool IsNumeric(this char? source)
    {
        if (source == null) return false;
        return source.Value.IsInteger();
    }

    public static bool IsInteger(this char source)
    {
        return "0123456789".Contains(source);
    }

    public static bool IsOperator(this char? source)
    {
        if (source == null) return false;
        return source.Value.IsOperator();
    }

    public static bool IsOperator(this char source)
    {
        return "+-*/#%&|".Contains(source);
    }

    public static bool IsSpecialSymbol(this char? source)
    {
        if (source == null) return false;
        return source.Value.IsSpecialSymbol();
    }

    public static bool IsSpecialSymbol(this char source)
    {
        return ",'();".Contains(source);
    }

    public static bool IsSpecialSymbol(char first, char? second)
    {
        if (second == null) return false;
        return IsSpecialSymbol(first, second.Value);
    }

    public static bool IsSpecialSymbol(char first, char second)
    {
        var symbols = new List<string>(){
            "--",
            "/*",
            "*/",
            "||",
            "!=",
            "<>",
            "<=",
            ">=",
            "::",
        };
        var s = ZString.Concat(first, second);
        return symbols.Contains(s);
    }

    public static bool IsDot(this char? source)
    {
        if (source == null) return false;
        return source.Value.IsDot();
    }

    public static bool IsDot(this char source)
    {
        return source == '.';
    }

    public static bool IsSingleQuote(this char source)
    {
        return source == '\'';
    }

    public static bool IsOpenBracket(this char? source)
    {
        if (source == null) return false;
        return source.Value.IsOpenBracket();
    }

    public static bool IsOpenBracket(this char source)
    {
        return source == '(';
    }

    public static bool IsCloseBracket(this char source)
    {
        return source == '(';
    }
}