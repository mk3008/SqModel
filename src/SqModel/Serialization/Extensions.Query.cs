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

    public static Query DecorateBracket(this Query source)
    {
        source.CommandText = $"({source.CommandText})";
        return source;
    }
}