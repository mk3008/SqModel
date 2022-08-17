using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class TableClauseJoin
{
    public static TableClause Join(this TableClause source, string tableName, string aliasName, RelationTypes type)
    {
        var d = new TableClause(tableName, aliasName);
        return source.Join(d, type);
    }

    public static TableClause Join(this TableClause source, SelectQuery subSelectClause, string aliasName, RelationTypes type)
    {
        var d = new TableClause(subSelectClause, aliasName);
        return source.Join(d, type);
    }

    public static TableClause Join(this TableClause source, TableClause destination, RelationTypes type)
    {
        destination.RelationConditionClause.SourceName = source.AliasName;
        destination.RelationConditionClause.DestinationName = destination.AliasName;
        destination.RelationType = type;
        source.SubTableClauses.Add(destination);
        return destination;
    }

    public static TableClause InnerJoin(this TableClause source, CommonTableClause ct)
    {
        return source.Join(ct.AliasName, ct.AliasName, RelationTypes.Inner);
    }

    public static TableClause InnerJoin(this TableClause source, string tableName)
    {
        return source.Join(tableName, tableName, RelationTypes.Inner);
    }

    public static TableClause InnerJoin(this TableClause source, string tableName, string aliasName)
    {
        return source.Join(tableName, aliasName, RelationTypes.Inner);
    }

    public static TableClause InnerJoin(this TableClause source, SelectQuery subSelectClause, string aliasName)
    {
        return source.Join(subSelectClause, aliasName, RelationTypes.Inner);
    }

    public static TableClause InnerJoin(this TableClause source, Action<SelectQuery> action, string aliasName)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Inner);
    }

    public static TableClause InnerJoin(this TableClause source, TableClause destination)
    {
        return source.Join(destination, RelationTypes.Inner);
    }

    public static TableClause LeftJoin(this TableClause source, string tableName)
    {
        return source.Join(tableName, tableName, RelationTypes.Left);
    }

    public static TableClause LeftJoin(this TableClause source, string tableName, string aliasName)
    {
        return source.Join(tableName, aliasName, RelationTypes.Left);
    }

    public static TableClause LeftJoin(this TableClause source, SelectQuery subSelectClause, string aliasName)
    {
        return source.Join(subSelectClause, aliasName, RelationTypes.Left);
    }

    public static TableClause LeftJoin(this TableClause source, Action<SelectQuery> action, string aliasName)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Left);
    }

    public static TableClause LeftJoin(this TableClause source, TableClause destination)
    {
        return source.Join(destination, RelationTypes.Left);
    }

    public static TableClause RightJoin(this TableClause source, string tableName)
    {
        return source.Join(tableName, tableName, RelationTypes.Right);

    }
    public static TableClause RightJoin(this TableClause source, string tableName, string aliasName)
    {
        return source.Join(tableName, aliasName, RelationTypes.Right);
    }

    public static TableClause RightJoin(this TableClause source, SelectQuery subSelectClause, string aliasName)
    {
        return source.Join(subSelectClause, aliasName, RelationTypes.Right);
    }

    public static TableClause RightJoin(this TableClause source, Action<SelectQuery> action, string aliasName)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Right);
    }

    public static TableClause RightJoin(this TableClause source, TableClause destination)
    {
        return source.Join(destination, RelationTypes.Right);
    }

    public static TableClause CrossJoin(this TableClause source, string tableName)
    {
        return source.Join(tableName, tableName, RelationTypes.Cross);
    }

    public static TableClause CrossJoin(this TableClause source, string tableName, string aliasName)
    {
        return source.Join(tableName, aliasName, RelationTypes.Cross);
    }

    public static TableClause CrossJoin(this TableClause source, SelectQuery subSelectClause, string aliasName)
    {
        return source.Join(subSelectClause, aliasName, RelationTypes.Cross);
    }

    public static TableClause CrossJoin(this TableClause source, Action<SelectQuery> action, string aliasName)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, aliasName, RelationTypes.Cross);
    }

    public static TableClause CrossJoin(this TableClause source, TableClause destination)
    {
        return source.Join(destination, RelationTypes.Cross);
    }

    public static TableClause On(this TableClause source, string column)
    {
        var fn = (TableClause x) => x.Where().Equal(column);
        source.On(fn);
        return source;
    }

    public static TableClause On(this TableClause source, string sourcecolumn, string destinationcolumn)
    {
        var fn = (TableClause x) => x.Where().Equal(sourcecolumn, destinationcolumn);
        source.On(fn);
        return source;
    }

    public static TableClause On(this TableClause source, Action<TableClause> fn)
    {
        fn(source);
        return source;
    }
}

