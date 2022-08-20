using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class Query
{
    public string CommandText { get; set; } = string.Empty;

    public Dictionary<string, object> Parameters { get; set; } = new();

    public Query Merge(Query query, string separator = " ")
    {
        var text = CommandText;
        if (CommandText.IsEmpty()) text = query.CommandText;
        else if (query.CommandText.IsNotEmpty()) text += $"{separator}{query.CommandText}";

        var prm = new Dictionary<string, object>();
        prm.Merge(Parameters);
        prm.Merge(query.Parameters);

        return new Query() { CommandText = text, Parameters = prm };
    }
}
