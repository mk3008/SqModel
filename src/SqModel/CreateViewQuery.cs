namespace SqModel;

public class CreateViewQuery
{
    public string ViewName { get; set; } = string.Empty;

    public bool IsTemporary { get; set; } = false;

    public SelectQuery SelectQuery { get; set; } = new();

    public Query ToQuery()
    {
        var q = SelectQuery.ToQuery();
        var tmp = (IsTemporary) ? "temporary " : "";
        q.CommandText = $"create {tmp}view {ViewName}\r\nas\r\n{q.CommandText}";
        return q;
    }
}
