﻿using SqModel.CommandContainer;
using SqModel.Extension;

namespace SqModel.Clause;

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

public enum RelationTypes
{
    Undefined = 0,
    From = 1,
    Inner = 2,
    Left = 3,
    Right = 4,
    Cross = 5,
}