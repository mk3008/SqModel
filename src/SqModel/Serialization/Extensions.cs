using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public static class Extensions
{
    public static bool IsEof(this int source) => source < 0;

    public static bool IsSpace(this char source) => Parser.SpaceTokens.Where(x => x == source).Any();

    public static bool IsSymbol(this char source) => Parser.SymbolTokens.Where(x => x == source).Any();

    public static bool IsLogicalOperator(this string source) => Parser.LogicalOperatorTokens.Where(x => x == source.ToLower()).Any();

    public static string TrimEndSpace(this string source) => source.TrimEnd(Parser.SpaceTokens.ToArray());

    //public static ReadTokenResult Trim(this ReadTokenResult source)
    //{
    //    source.Token = source.Token.TrimEndSpace();
    //    return source;
    //}

    public static bool IsLetter(this string source)
    {
        if (source.Length == 0) return false;
        var c = source.ToCharArray().First();
        return Parser.LetterChars.Where(x => x == c).Any();
    }

    public static bool IsSpace(this char? source) => (source == null) ? false : source.Value.IsSpace();

    public static bool IsSymbol(this char? source) => (source == null) ? false : source.Value.IsSymbol();

    public static bool IsToken(this StringBuilder source, IEnumerable<string> tokens)
    {
        var s = source.ToString().ToLower();
        return tokens.Where(x => x == s).Any();
    }

    public static bool IsMaybeToken(this StringBuilder source, IEnumerable<string> tokens)
    {
        var s = source.ToString().ToLower();
        return tokens.Where(x => x.IndexOf(s) == 0).Any();
    }

    public static Query InsertToken(this Query source, string token, string splitter = " ")
    {
        source.CommandText = $"{token}{splitter}{source.CommandText}";
        return source;
    }

    public static Query DecorateBracket(this Query source)
    {
        source.CommandText = $"({source.CommandText})";
        return source;
    }
}