using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class TableClauseExtension
{
    public static TableClause Join(this TableClause source, string tableName, string aliasName, RelationTypes type)
        => source.Join(new TableClause(tableName, aliasName), type);


    public static TableClause Join(this TableClause source, SelectQuery subSelectClause, string aliasName, RelationTypes type)
        => source.Join(new TableClause(subSelectClause, aliasName), type);

    public static TableClause Join(this TableClause source, TableClause destination, RelationTypes type)
    {
        destination.RelationConditionClause.SourceName = source.AliasName;
        destination.RelationConditionClause.DestinationName = destination.AliasName;
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

    public static TableClause InnerJoin(this TableClause source, CommonTableClause ct)
        => source.Join(ct.AliasName, ct.AliasName, RelationTypes.Inner);

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

    public static TableClause LeftJoin(this TableClause source, CommonTableClause ct)
        => source.Join(ct.AliasName, ct.AliasName, RelationTypes.Left);

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

    public static TableClause RightJoin(this TableClause source, CommonTableClause ct)
        => source.Join(ct.AliasName, ct.AliasName, RelationTypes.Right);

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

    public static TableClause CrossJoin(this TableClause source, CommonTableClause ct)
        => source.Join(ct.AliasName, ct.AliasName, RelationTypes.Cross);

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
        var fn = (RelationContainer x) => x.Add().Equal(sourcecolumn, destinationcolumn);
        source.On(fn);
        return source;
    }

    public static TableClause On(this TableClause source, Action<RelationContainer> fn)
    {
        fn(source.Where);
        return source;
    }
}

