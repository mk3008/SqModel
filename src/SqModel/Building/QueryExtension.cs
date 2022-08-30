using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SqModel.Extension;

namespace SqModel.Building;

internal static class QueryExtension
{
    public static Query InsertToken(this Query source, string token, string splitter = " ")
    {
        if (token.IsEmpty()) return source;
        source.CommandText = $"{token}{splitter}{source.CommandText}";
        return source;
    }

    public static Query AddToken(this Query source, string token, string splitter = " ")
    {
        if (token.IsEmpty()) return source;
        source.CommandText = $"{source.CommandText}{splitter}{token}";
        return source;
    }

    public static Query Decorate(this Query source, string prefix, string sufix, string splitter = " ")
    {
        source.CommandText = $"{prefix}{splitter}{source.CommandText}{splitter}{sufix}";
        return source;
    }

    public static Query DecorateBracket(this Query source, string splitter = "")
        => source.Decorate("(", ")", splitter);

    public static Query InsertIndent(this Query source, string separator = "\r\n", int spaceCount = 4)
    {
        source.CommandText = source.CommandText.InsertIndent(separator, spaceCount);
        return source;
    }
}