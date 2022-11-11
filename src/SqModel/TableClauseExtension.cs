using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;


public static class TableClauseExtension
{
    public static List<TableClause> GetTableClauses(this TableClause source)
    {
        var lst = new List<TableClause>();

        lst.Add(source);
        source.SubTableClauses.ForEach(x => lst.AddRange(x.GetTableClauses()));

        return lst;
    }

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

    public static TableClause InnerJoin(this TableClause source, TableClause destination)
        => source.Join(destination, RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, SelectQuery subSelectClause)
        => source.Join(subSelectClause, "", RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, CommonTable ct)
        => source.Join(ct.Name, ct.Name, RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, Func<SelectQuery> fn)
        => source.Join(fn(), "", RelationTypes.Inner);

    public static TableClause InnerJoin(this TableClause source, Action<SelectQuery> action)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, "", RelationTypes.Inner);
    }

    public static TableClause LeftJoin(this TableClause source, string tableName)
        => source.Join(tableName, tableName, RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, TableClause destination)
        => source.Join(destination, RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, SelectQuery subSelectClause)
        => source.Join(subSelectClause, "", RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, CommonTable ct)
        => source.Join(ct.Name, ct.Name, RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, Func<SelectQuery> fn)
        => source.Join(fn(), "", RelationTypes.Left);

    public static TableClause LeftJoin(this TableClause source, Action<SelectQuery> action)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, "", RelationTypes.Left);
    }

    public static TableClause RightJoin(this TableClause source, string tableName)
        => source.Join(tableName, tableName, RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, TableClause destination)
        => source.Join(destination, RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, SelectQuery subSelectClause)
        => source.Join(subSelectClause, "", RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, CommonTable ct)
        => source.Join(ct.Name, ct.Name, RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, Func<SelectQuery> fn)
        => source.Join(fn(), "", RelationTypes.Right);

    public static TableClause RightJoin(this TableClause source, Action<SelectQuery> action)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, "", RelationTypes.Right);
    }

    public static TableClause CrossJoin(this TableClause source, string tableName)
        => source.Join(tableName, tableName, RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, TableClause destination)
        => source.Join(destination, RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, SelectQuery subSelectClause)
        => source.Join(subSelectClause, "", RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, CommonTable ct)
        => source.Join(ct.Name, ct.Name, RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, Func<SelectQuery> fn)
        => source.Join(fn(), "", RelationTypes.Cross);

    public static TableClause CrossJoin(this TableClause source, Action<SelectQuery> action)
    {
        var subSelectClause = new SelectQuery();
        action(subSelectClause);
        return source.Join(subSelectClause, "", RelationTypes.Cross);
    }

    public static TableClause As(this TableClause source, string aliasName)
    {
        source.AliasName = aliasName;
        source.RelationClause.RightTable = source.AliasName;
        return source;
    }

    public static TableClause On(this TableClause source, string column)
        => source.On(column, column);

    public static TableClause On(this TableClause source, List<string> columns)
    {
        source.On(g =>
        {
            g.IsDecorateBracket = false;
            columns.ForEach(x => g.Add().Column(g.LeftTable, x).Equal(g.RightTable, x));
        });
        return source;
    }

    public static TableClause On(this TableClause source, string sourcecolumn, string destinationcolumn)
    {
        var st = source.RelationClause.LeftTable;
        var dt = source.RelationClause.RightTable;
        source.On.Add().Column(st, sourcecolumn).Equal(dt, destinationcolumn);
        return source;
    }

    public static TableClause On(this TableClause source, Action<ConditionGroup> fn)
    {
        var r = source.RelationClause;
        var g = new ConditionGroup() { LeftTable = r.LeftTable, RightTable = r.RightTable, IsDecorateBracket = false };
        source.RelationClause.Collection.Add(g);
        fn(g);
        return source;
    }
}