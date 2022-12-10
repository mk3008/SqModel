using SqModel.Extension;

namespace SqModel;

public class InsertQuery
{
    public string TableName { get; set; } = string.Empty;

    public SelectQuery SelectQuery { get; set; } = new();

    public Query ToQuery()
    {
        var q = SelectQuery.ToQuery();
        var cols = SelectQuery.Select.GetColumnNames();

        //If you are using wildcards, omit the column clause.
        var coltext = !cols.Any() ? "" : $"({cols.ToString(", ")})";

        q.CommandText = $"insert into {TableName}{coltext}\r\n{q.CommandText}";
        return q;
    }
}