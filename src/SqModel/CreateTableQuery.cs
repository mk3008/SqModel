using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class CreateTableQuery
{
    public string TableName { get; set; } = string.Empty;

    public bool IsTemporary { get; set; } = false;

    public SelectQuery SelectQuery { get; set; } = new();

    public Query ToQuery()
    {
        var q = SelectQuery.ToQuery();
        var tmp = (IsTemporary) ? "temporary " : "";
        q.CommandText = $"create {tmp}table {TableName}\r\nas\r\n{q.CommandText}";
        return q;
    }
}
