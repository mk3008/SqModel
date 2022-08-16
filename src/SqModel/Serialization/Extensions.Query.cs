using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;

internal static partial class Extensions
{
    public static Query InsertToken(this Query source, string token, string splitter = " ")
    {
        source.CommandText = $"{token}{splitter}{source.CommandText}";
        return source;
    }

    public static Query DecorateBracket(this Query source, string splitter = "")
    {
        source.CommandText = $"({splitter}{source.CommandText}{splitter})";
        return source;
    }

    public static Query InsertIndent(this Query source, string separator = "\r\n", int spaceCount = 4)
    {
        source.CommandText = source.CommandText.InsertIndent(separator, spaceCount);
        return source;
    }
}