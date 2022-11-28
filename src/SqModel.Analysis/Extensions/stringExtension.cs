using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Analysis.Extensions;

public static class stringExtension
{
    public static bool AreEqual(this string source, string text)
    {
        return string.Equals(source, text, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool AreContains(this string source, IEnumerable<string> texts)
    {
        return texts.Where(x => source.AreEqual(x)).Any();
    }

    public static bool IsDot(this string source)
    {
        return source == ".";
    }

    public static bool IsSelectQuery(this string source)
    {
        return Regex.IsMatch(source, @"^select\s", RegexOptions.IgnoreCase);
    }

    public static bool IsNumeric(this string source)
    {
        if (string.IsNullOrEmpty(source)) return false;
        return source.First().IsInteger();
    }

    public static bool IsOpenBracket(this string source)
    {
        if (string.IsNullOrEmpty(source)) return false;
        return source == "(";
    }

    public static bool IsSingleQuote(this string source)
    {
        if (string.IsNullOrEmpty(source)) return false;
        return source == "'";
    }

    public static bool IsOperator(this string source)
    {
        if (string.IsNullOrEmpty(source)) return false;
        if (source.First().IsOperator()) return true;
        if (source.AreEqual("and")) return true;
        if (source.AreEqual("or")) return true;
        return false;
    }

    //public static bool Is2WordToken(string first, string second)
    //{
    //    var tokens = new List<string>(){
    //        "inner join",
    //        "left join",
    //        "right join",
    //        "cross join",
    //        "case when",
    //        "partition by",
    //        "order by",
    //        "union all",
    //        "is not",
    //        "not materialized",
    //    };
    //    var s = $"{first} {second}";
    //    return s.AreContains(tokens);
    //}

    public static string ToCommandText(this List<string> source)
    {
        var sb = ZString.CreateStringBuilder();
        var prev = string.Empty;
        source.ForEach(x =>
        {
            if (sb.Length == 0)
            {
                sb.Append(x);
            }
            else if (prev.ToLower() == "exists")
            {
                sb.Append($" {x}");
            }
            else if (prev == "." || prev == "(")
            {
                sb.Append(x);
            }
            else if (x == "." || x.StartsWith("(") || x == ")" || x.StartsWith("::"))
            {
                sb.Append(x);
            }
            else
            {
                sb.Append($" {x}");
            }
            prev = x;
        });
        return sb.ToString();
    }
}