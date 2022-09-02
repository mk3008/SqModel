using SqModel;
using SqModel.Extension;

namespace SqModel;

public class TableClause
{
    public TableClause() { }

    public TableClause(string tableName)
    {
        TableName = tableName;
        AliasName = tableName;
    }

    public TableClause(string tableName, string aliasName)
    {
        TableName = tableName;
        AliasName = aliasName;
    }

    public TableClause(SelectQuery subSelectClause, string aliasName)
    {
        SubSelectClause = subSelectClause;
        AliasName = aliasName;
    }

    public RelationTypes RelationType { get; set; } = RelationTypes.Undefined;

    public string TableName { get; set; } = string.Empty;

    public SelectQuery? SubSelectClause { get; set; } = null;

    public string AliasName { get; set; } = string.Empty;

    public string SourceAlias { get; set; } = string.Empty;

    public RelationGroup RelationClause { get; set; } = new RelationGroup() { IsDecorateBracket = false };

    public RelationGroup On => RelationClause;

    public List<TableClause> SubTableClauses { get; set; } = new();

    public string GetName() => AliasName != string.Empty ? AliasName : TableName;

    public string GetAliasCommand() => TableName != GetName() || SubSelectClause != null ? $" as {GetName()}" : string.Empty;

    public Query ToQuery()
    {
        var q = ToCurrentQuery();
        var subQ = SubTableClauses.Select(x => x.ToQuery()).ToList();
        subQ.ForEach(x => q = q.Merge(x, "\r\n"));
        return q;
    }

    public Query ToCurrentQuery()//string alias, Query tableQ
    {
        var join = string.Empty;

        switch (RelationType)
        {
            case RelationTypes.From:
                join = "from ";
                break;
            case RelationTypes.Inner:
                join = "inner join ";
                break;
            case RelationTypes.Left:
                join = "left join ";
                break;
            case RelationTypes.Right:
                join = "right join ";
                break;
            case RelationTypes.Cross:
                join = "cross join ";
                break;
        }

        var fnQuery = () =>
        {
            if (SubSelectClause != null)
            {
                SubSelectClause.IsincludeCte = false;
                var sq = SubSelectClause.ToQuery();
                var text = $"{join}(\r\n{sq.CommandText.InsertIndent()}\r\n){GetAliasCommand()}";
                return new Query() { CommandText = text, Parameters = sq.Parameters };
            }
            else
            {
                var text = $"{join}{TableName}{GetAliasCommand()}";
                return new Query() { CommandText = text, Parameters = new() };
            }
        };

        var q = fnQuery();
        if (RelationType == RelationTypes.From || RelationType == RelationTypes.Cross) return q;

        return q.Merge(RelationClause.ToQuery(), " on ");
    }

    public IEnumerable<CommonTable> GetCommonTableClauses()
    {
        foreach (var x in SubTableClauses) foreach (var item in x.GetCommonTableClauses()) yield return item;

        var lst = SubSelectClause?.GetAllWith().CommonTableAliases.ToList();
        if (SubSelectClause != null) foreach (var item in SubSelectClause.GetAllWith().CommonTableAliases) yield return item;
    }
}

public static class TableClauseExtension
{
    public static TableClause Join(this TableClause source, string tableName, string aliasName, RelationTypes type)
        => source.Join(new TableClause(tableName, aliasName), type);

    public static TableClause Join(this TableClause source, SelectQuery subSelectClause, string aliasName, RelationTypes type)
        => source.Join(new TableClause(subSelectClause, aliasName), type);

    public static TableClause Join(this TableClause source, TableClause destination, RelationTypes type)
    {
        destination.RelationClause.LeftTable = source.AliasName;
        destination.RelationClause.RightTable = destination.AliasName;
        destination.RelationType = type;
        source.SubTableClauses.Add(destination);
        return destination;
    }

    public static TableClause InnerJoin(this TableClause source, string tableName)
        => source.Join(tableName, tableName, RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, string tableName, string aliasName)
        => source.Join(tableName, aliasName, RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, TableClause destination)
        => source.Join(destination, RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, SelectQuery subSelectClause, string aliasName)
        => source.Join(subSelectClause, aliasName, RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, CommonTable ct)
        => source.Join(ct.Name, ct.Name, RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, Func<SelectQuery> fn, string aliasName)
        => source.Join(fn(), aliasName, RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, Action<SelectQuery> action, string aliasName)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Inner);
    }

    public static TableClause LeftJoin(this TableClause source, string tableName)
        => source.Join(tableName, tableName, RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, string tableName, string aliasName)
        => source.Join(tableName, aliasName, RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, TableClause destination)
        => source.Join(destination, RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, SelectQuery subSelectClause, string aliasName)
        => source.Join(subSelectClause, aliasName, RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, CommonTable ct)
        => source.Join(ct.Name, ct.Name, RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, Func<SelectQuery> fn, string aliasName)
        => source.Join(fn(), aliasName, RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, Action<SelectQuery> action, string aliasName)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Left);
    }

    public static TableClause RightJoin(this TableClause source, string tableName)
        => source.Join(tableName, tableName, RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, string tableName, string aliasName)
        => source.Join(tableName, aliasName, RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, TableClause destination)
        => source.Join(destination, RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, SelectQuery subSelectClause, string aliasName)
        => source.Join(subSelectClause, aliasName, RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, CommonTable ct)
        => source.Join(ct.Name, ct.Name, RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, Func<SelectQuery> fn, string aliasName)
        => source.Join(fn(), aliasName, RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, Action<SelectQuery> action, string aliasName)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Right);
    }

    public static TableClause CrossJoin(this TableClause source, string tableName)
        => source.Join(tableName, tableName, RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, string tableName, string aliasName)
        => source.Join(tableName, aliasName, RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, TableClause destination)
        => source.Join(destination, RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, SelectQuery subSelectClause, string aliasName)
        => source.Join(subSelectClause, aliasName, RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, CommonTable ct)
        => source.Join(ct.Name, ct.Name, RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, Func<SelectQuery> fn, string aliasName)
        => source.Join(fn(), aliasName, RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, Action<SelectQuery> action, string aliasName)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Cross);
    }

    public static TableClause On(this TableClause source, string column)
        => source.On(column, column);

    public static TableClause On(this TableClause source, string sourcecolumn, string destinationcolumn)
    {
        var st = source.RelationClause.LeftTable;
        var dt = source.RelationClause.RightTable;
        source.On.Add().Column(st, sourcecolumn).Equal(dt, destinationcolumn);
        return source;
    }

    public static TableClause On(this TableClause source, Action<RelationGroup> fn)
    {
        var r = source.RelationClause;
        var g = new RelationGroup() { LeftTable = r.LeftTable, RightTable = r.RightTable };
        source.RelationClause.Collection.Add(g);
        fn(g);
        return source;
    }
}