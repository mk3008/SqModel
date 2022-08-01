using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class TableClauseJoin
{
    public static TableClause Join(this TableClause source, string tableName, string aliasName, RelationTypes type, Dictionary<string, string> keyvalues)
    {
        var d = new TableClause(tableName, aliasName);
        return source.Join(d, type, keyvalues);
    }

    public static TableClause Join(this TableClause source, SelectQuery subSelectClause, string aliasName, RelationTypes type, Dictionary<string, string> keyvalues)
    {
        var d = new TableClause(subSelectClause, aliasName);
        return source.Join(d, type, keyvalues);
    }

    public static TableClause Join(this TableClause source, TableClause destination, RelationTypes type, Dictionary<string, string> keyvalues)
    {
        destination.SourceAlias = source.AliasName;
        destination.RelationType = type;
        keyvalues.ToList().ForEach(x => destination.Add(x.Key, x.Value));
        source.SubTableClauses.Add(destination);
        return destination;
    }

    public static TableClause InnerJoin(this TableClause source, string tableName, List<string> columns)
    {
        return source.Join(tableName, tableName, RelationTypes.Inner, columns.ToDictionary());
    }

    public static TableClause InnerJoin(this TableClause source, string tableName, string aliasName, List<string> columns)
    {
        return source.Join(tableName, aliasName, RelationTypes.Inner, columns.ToDictionary());
    }

    public static TableClause InnerJoin(this TableClause source, SelectQuery subSelectClause, string aliasName, List<string> columns)
    {
        return source.Join(subSelectClause, aliasName, RelationTypes.Inner, columns.ToDictionary());
    }

    public static TableClause InnerJoin(this TableClause source, Action<SelectQuery> action, string aliasName, List<string> columns)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Inner, columns.ToDictionary());
    }

    public static TableClause InnerJoin(this TableClause source, TableClause destination, List<string> columns)
    {
        return source.Join(destination, RelationTypes.Inner, columns.ToDictionary());
    }

    public static TableClause LeftJoin(this TableClause source, string tableName, List<string> columns)
    {
        return source.Join(tableName, tableName, RelationTypes.Left, columns.ToDictionary());
    }

    public static TableClause LeftJoin(this TableClause source, string tableName, string aliasName, List<string> columns)
    {
        return source.Join(tableName, aliasName, RelationTypes.Left, columns.ToDictionary());
    }

    public static TableClause LeftJoin(this TableClause source, SelectQuery subSelectClause, string aliasName, List<string> columns)
    {
        return source.Join(subSelectClause, aliasName, RelationTypes.Left, columns.ToDictionary());
    }

    public static TableClause LeftJoin(this TableClause source, Action<SelectQuery> action, string aliasName, List<string> columns)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Left, columns.ToDictionary());
    }

    public static TableClause LeftJoin(this TableClause source, TableClause destination, List<string> columns)
    {
        return source.Join(destination, RelationTypes.Left, columns.ToDictionary());
    }

    public static TableClause RightJoin(this TableClause source, string tableName, List<string> columns)
    {
        return source.Join(tableName, tableName, RelationTypes.Right, columns.ToDictionary());

    }
    public static TableClause RightJoin(this TableClause source, string tableName, string aliasName, List<string> columns)
    {
        return source.Join(tableName, aliasName, RelationTypes.Right, columns.ToDictionary());
    }

    public static TableClause RightJoin(this TableClause source, SelectQuery subSelectClause, string aliasName, List<string> columns)
    {
        return source.Join(subSelectClause, aliasName, RelationTypes.Right, columns.ToDictionary());
    }

    public static TableClause RightJoin(this TableClause source, Action<SelectQuery> action, string aliasName, List<string> columns)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Right, columns.ToDictionary());
    }

    public static TableClause RightJoin(this TableClause source, TableClause destination, List<string> columns)
    {
        return source.Join(destination, RelationTypes.Right, columns.ToDictionary());
    }

    public static TableClause CrossJoin(this TableClause source, string tableName)
    {
        return source.Join(tableName, tableName, RelationTypes.Cross, new());
    }

    public static TableClause CrossJoin(this TableClause source, string tableName, string aliasName)
    {
        return source.Join(tableName, aliasName, RelationTypes.Cross, new());
    }

    public static TableClause CrossJoin(this TableClause source, SelectQuery subSelectClause, string aliasName)
    {
        return source.Join(subSelectClause, aliasName, RelationTypes.Cross, new());
    }

    public static TableClause CrossJoin(this TableClause source, Action<SelectQuery> action, string aliasName)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Cross, new());
    }

    public static TableClause CrossJoin(this TableClause source, TableClause destination)
    {
        return source.Join(destination, RelationTypes.Cross, new());
    }
}

