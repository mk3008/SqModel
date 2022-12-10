namespace SqModel;

public static class SelectQueryFromExtension
{
    public static TableClause From(this SelectQuery source, CommonTable ct)
    {
        source.FromClause = new TableClause() { TableName = ct.Name, AliasName = ct.Name, RelationType = RelationTypes.From, Actual = ct.Query };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, string tableName)
    {
        source.FromClause = new TableClause() { TableName = tableName, AliasName = tableName, RelationType = RelationTypes.From };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, SelectQuery subquery)
    {
        source.FromClause = new TableClause() { SubSelectClause = subquery, RelationType = RelationTypes.From, Actual = subquery };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, Action<SelectQuery> action)
    {
        var q = new SelectQuery();
        action(q);
        source.FromClause = new TableClause() { SubSelectClause = q, RelationType = RelationTypes.From, Actual = q };
        return source.FromClause;
    }
}

