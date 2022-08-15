using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public static class Extensions
{
    public static bool IsEof(this int source)
        => source < 0;

    public static bool IsSpace(this char source)
        => Parser.SpaceTokens.Where(x => x == source).Any();

    public static bool IsSymbol(this char source)
        => Parser.SymbolTokens.Where(x => x == source).Any();

    public static bool IsLogicalOperator(this string source) 
        => Parser.LogicalOperatorTokens.Where(x => x == source.ToLower()).Any();

    public static bool IsLetter(this string source)
    {
        if (source.Length == 0) return false;
        var c = source.ToCharArray().First();
        return Parser.LetterChars.Where(x => x == c).Any();
    }

    public static bool Any(this IEnumerable<string> source, string  token) 
        => source.Where(x => x == token.ToLower()).Any();

    public static bool Any(this string source, char token) 
        => source.ToArray().Where(x => x == token).Any();

    public static bool IsSpace(this char? source)
        => (source == null) ? false : source.Value.IsSpace();

    public static bool IsSymbol(this char? source) 
        => (source == null) ? false : source.Value.IsSymbol();

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

    public static RelationTypes ToRelationType(this string source)
    {
        if (source.IsFromRealtion()) return RelationTypes.From;
        else if (source.IsInnerJoinRealtion()) return RelationTypes.Inner;
        else if (source.IsLeftJoinRelation()) return RelationTypes.Left;
        else if (source.IsRightJoinRealtion()) return RelationTypes.Right;
        else if (source.IsCrossJoinRealtion()) return RelationTypes.Cross;
        return RelationTypes.Undefined;
    }

    public static bool IsFromRealtion(this string source)
        => Parser.FromTokens.Where(x => x == source.ToLower()).Any();
    public static bool IsInnerJoinRealtion(this string source) 
        => Parser.InnerJoinTokens.Where(x => x == source.ToLower()).Any();
    public static bool IsLeftJoinRelation(this string source) 
        => Parser.LeftJoinTokens.Where(x => x == source.ToLower()).Any();
    public static bool IsRightJoinRealtion(this string source) 
        => Parser.RightJoinTokens.Where(x => x == source.ToLower()).Any();
    public static bool IsCrossJoinRealtion(this string source) 
        => Parser.CrossJoinTokens.Where(x => x == source.ToLower()).Any();
}