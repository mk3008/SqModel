using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public static class Extensions
{
    //public static bool IsLowerMatch(this char source, char lowerChar)
    //{
    //    return char.ToLower(source) == lowerChar;
    //}

    public static bool IsFirstChar(this string source, char c)
    {
        return (source.IndexOf(c.ToLower()) == 0 || source.IndexOf(c.ToUpper()) == 0);
    }

    public static char ToLower(this char source) => char.ToLower(source);
    public static char ToUpper(this char source) => char.ToUpper(source);

    public static bool IsEof(this int source) => source < 0;

    public static bool IsNotEof(this int source) => !source.IsEof();

    public static char ToChar(this int source) => (char)source;

    public static bool IsSpace(this char source) => " \r\n\t;".IndexOf(source) >= 0;

    public static bool IsSpace(this char? source) => (source == null) ? true : source.Value.IsSpace();

    public static bool Contains(this IEnumerable<string> source,string value, Func<string, string> converter) => source.Select(x => converter(x)).Contains(value);


}