﻿using Carbunql.Core.Values;
using Cysharp.Text;
using System.Text;

namespace Carbunql.Core.Extensions;

public static class IEnumerableExtension
{
    public static string ToText(this IEnumerable<Token> source)
    {
        var sb = ZString.CreateStringBuilder();
        Token? prev = null;

        foreach (var item in source)
        {
            if (string.IsNullOrEmpty(item.Text)) continue;
            if (item.NeedsSpace(prev))
            {
                sb.Append(" " + item.Text);
            }
            else
            {
                sb.Append(item.Text);
            }
            prev = item;
        }
        return sb.ToString();
    }
}