using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class CommonTableClause
{
    //public string CommandText { get; set; } = String.Empty;

    public Dictionary<string, object> Parameters { get; set; } = new();

    public SelectQuery SelectQuery { get; set; } = new();

    public string AliasName { get; set; } = string.Empty;

    public Query ToQuery()
    {
        var q = SelectQuery.ToSubQuery();
        q.CommandText = $"{AliasName} as (\r\n{q.CommandText.InsertIndent()}\r\n)";

        return q;
    }
}
