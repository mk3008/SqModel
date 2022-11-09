using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Extension;

public static class stringExtension
{
    public static string InsertIndent(this string source, string separator = "\r\n", int spaceCount = 4)
    {
        if (source.IsEmpty()) return source;

        var indent = spaceCount.ToSpaceString();

        return $"{indent}{source.Replace(separator, $"{separator}{indent}")}";
    }

    public static bool IsEmpty(this string? source)
        => string.IsNullOrEmpty(source);

    public static bool IsNotEmpty(this string? source)
        => !string.IsNullOrEmpty(source);

    public static Dictionary<string, string> ToDictionary(this List<string> source)
    {
        var dic = new Dictionary<string, string>();
        source.ForEach(x => dic.Add(x, x));
        return dic;
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
        => SqlParser.FromTokens.Where(x => x == source.ToLower()).Any();

    public static bool IsInnerJoinRealtion(this string source)
        => SqlParser.InnerJoinTokens.Where(x => x == source.ToLower()).Any();

    public static bool IsLeftJoinRelation(this string source)
        => SqlParser.LeftJoinTokens.Where(x => x == source.ToLower()).Any();

    public static bool IsRightJoinRealtion(this string source)
        => SqlParser.RightJoinTokens.Where(x => x == source.ToLower()).Any();

    public static bool IsCrossJoinRealtion(this string source)
        => SqlParser.CrossJoinTokens.Where(x => x == source.ToLower()).Any();

    public static bool Any(this IEnumerable<string> source, string token)
        => source.Where(x => x == token.ToLower()).Any();

    public static bool Any(this string source, char token)
        => source.ToArray().Where(x => x == token).Any();

    public static bool IsEof(this int source)
        => source < 0;

    public static bool IsLogicalOperator(this string source)
        => SqlParser.LogicalOperatorTokens.Where(x => x == source.ToLower()).Any();

    public static bool IsLetter(this string source)
    {
        if (source.Length == 0) return false;
        var c = source.ToCharArray().First();
        return SqlParser.LetterChars.Where(x => x == c).Any();
    }

    public static bool IsWord(this string source)
    {
        if (source.Length == 0) return false;
        return !(source.ToCharArray().ToList().Where(x => !SqlParser.LetterChars.Contains(x)).Any());
    }

    public static string ToSnakeCase(this string source)
    {
        var sb = new StringBuilder();
        source.ToList().ForEach(x =>
        {
            if (sb.Length != 0 && x.ToString() == x.ToString().ToUpper()) sb.Append("_");
            sb.Append(x);
        });
        return sb.ToString();
    }

    public static bool IsConjunction(this string source)
    {
        if (SqlParser.ArithmeticOperatorTokens.Contains(source) || source == "||") return true; // || source == "=" || source == "!=" || source == "<>" || source.ToLower() == "is") return true;
        return false;
    }
}