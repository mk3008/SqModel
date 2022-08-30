using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Extension;

namespace SqModel;

public class Query
{
    public string CommandText { get; set; }= string.Empty;

    public Dictionary<string, object> Parameters { get; set; } = new();

    //internal Query? Decorate(string prefixToken, string sufixToken)
    //{
    //    throw new NotImplementedException();
    //}
}

internal static class QueryExtension
{
    public static Query Merge(this Query source, Query query, string separator = " ")
    {
        var text = source.CommandText;
        if (source.CommandText.IsEmpty()) text = query.CommandText;
        else if (query.CommandText.IsNotEmpty()) text += $"{separator}{query.CommandText}";

        var prm = new Dictionary<string, object>();
        prm.Merge(source.Parameters);
        prm.Merge(query.Parameters);

        return new Query() { CommandText = text, Parameters = prm };
    }

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
