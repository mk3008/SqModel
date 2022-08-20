using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public partial class SelectQuery
{
    public Query ToCreateTableQuery(string tablename, bool istemporary = true)
    {
        var q = ToQuery();
        var tmp = (istemporary) ? "temporary " : "";
        q.CommandText = $"create {tmp}table {tablename}\r\nas\r\n{q.CommandText}";
        return q;
    }

    public Query ToInsertQuery(string tablename)
    {
        var q = ToQuery();
        var cols = SelectClause.GetColumnNames();
        var coltext = $"({cols.ToString(", ")})";

        q.CommandText = $"insert into {tablename}{coltext}\r\n{q.CommandText}";
        return q;
    }
}
