using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

internal static partial class Extensions
{
    public static Query ToQuery(this List<Query> source, string separator)
    {
        var text = source.Select(x => x.CommandText).ToList().ToString(separator);
        var prm = new Dictionary<string, object>();
        source.ForEach(x => prm.Merge(x.Parameters));

        return new Query() { CommandText = text, Parameters = prm };
    }

    public static bool IsEmpty(this Query? source)
        => string.IsNullOrEmpty(source?.CommandText);

    public static bool IsNotEmpty(this Query? source)
        => !string.IsNullOrEmpty(source?.CommandText);
}
