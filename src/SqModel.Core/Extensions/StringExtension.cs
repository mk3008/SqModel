﻿using System.Text;

namespace SqModel.Core.Extensions;

public static class StringExtension
{
    public static bool AreEqual(this string? source, string text)
    {
        return string.Equals(source, text, StringComparison.CurrentCultureIgnoreCase);
    }

    public static string InsertIndent(this string source, string separator = "\r\n", int spaceCount = 4)
    {
        if (string.IsNullOrEmpty(source)) return source;

        var indent = spaceCount.ToSpaceString();

        return $"{indent}{source.Replace(separator, $"{separator}{indent}")}";
    }

    public static string Join(this string source, string jointext, IEnumerable<string>? items, string splitter = ",", string? startDecorate = null, string? endDecorate = null)
    {
        if (items == null || !items.Any()) return source;

        var sb = new StringBuilder();
        sb.Append($"{source}{jointext}");
        sb.Append(items.ToString(splitter, startDecorate, endDecorate));
        return sb.ToString();
    }

    public static string ToString(this IEnumerable<string> source, string splitter = ",", string? startDecorate = null, string? endDecorate = null)
    {
        if (!source.Any()) return string.Empty;

        var sb = new StringBuilder();
        var isFirst = true;

        if (startDecorate != null) sb.Append(startDecorate);
        foreach (var item in source)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                sb.Append(splitter);
            }
            sb.Append(item);
        }
        if (endDecorate != null) sb.Append(endDecorate);

        return sb.ToString();
    }
}