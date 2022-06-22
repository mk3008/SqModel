using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SqModel.TableRelationClause;

namespace SqModel;

/// <summary>
/// Table clause.
/// Define the table to be acquired and the table associated with it.
/// </summary>
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

    public string TableName { get; set; } = String.Empty;

    public SelectQuery? SubSelectClause { get; set; } = null;

    public string AliasName { get; set; } = string.Empty;

    public TableRelationClause? TableRelationClause { get; set; }

    public List<TableClause> SubTableClauses { get; set; } = new();

    public IEnumerable<CommonTableClause> GetCommonTableClauses()
    {
        foreach (var x in SubTableClauses) foreach (var item in x.GetCommonTableClauses()) yield return item;

        var lst = SubSelectClause?.GetAllWith().CommonTableAliases.ToList();
        if (SubSelectClause != null) foreach (var item in SubSelectClause.GetAllWith().CommonTableAliases) yield return item;
    }

    public Query ToQuery(bool isRoot = true)
    {
        var q = (isRoot) ? ToFromQuery() : ToJoinQuery();

        var nestQ = SubTableClauses.Select(x => x.ToQuery(false)).ToList();
        nestQ.ForEach(x => q = q.Merge(x, "\r\n"));

        return q;
    }
    private Query ToFromQuery()
    {
        var tableQ = ToTableQuery();
        tableQ.CommandText = $"from {tableQ.CommandText}";
        return tableQ;
    }

    private Query ToJoinQuery()
    {
        var tableQ = ToTableQuery();
        if (TableRelationClause == null) throw new InvalidOperationException();
        return TableRelationClause.ToQuery(AliasName, tableQ);
    }

    private Query ToTableQuery()
    {
        if (TableName != string.Empty) return ToTableQueryByTable();
        return ToTableQueryBySubSelectClause();
    }

    private Query ToTableQueryByTable()
    {
        if (TableName == string.Empty) throw new InvalidOperationException("TableName is required");
        if (AliasName == string.Empty) throw new InvalidOperationException("AliasName is required");

        if (TableName == AliasName) return new Query() { CommandText = TableName, Parameters = new() };

        var text = $"{TableName} as {AliasName}";
        return new Query() { CommandText = text, Parameters = new() };
    }

    private Query ToTableQueryBySubSelectClause()
    {
        if (AliasName == string.Empty) throw new InvalidOperationException("AliasName is required");
        if (SubSelectClause == null) throw new InvalidOperationException("SubSelectClause is required");

        var q = SubSelectClause.ToSubQuery();

        var text = (q.CommandText == AliasName) ? AliasName : $"{q.CommandText} as {AliasName}";
        return new Query() { CommandText = text, Parameters = q.Parameters };
    }
}