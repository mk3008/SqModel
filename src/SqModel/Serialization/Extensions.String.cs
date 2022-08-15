using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

internal static partial class Extensions
{
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

    public static bool Any(this IEnumerable<string> source, string token)
        => source.Where(x => x == token.ToLower()).Any();

    public static bool Any(this string source, char token)
        => source.ToArray().Where(x => x == token).Any();

    public static bool IsEof(this int source)
        => source < 0;

    public static bool IsLogicalOperator(this string source)
        => Parser.LogicalOperatorTokens.Where(x => x == source.ToLower()).Any();

    public static bool IsLetter(this string source)
    {
        if (source.Length == 0) return false;
        var c = source.ToCharArray().First();
        return Parser.LetterChars.Where(x => x == c).Any();
    }
}
