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

    public List<TableClause> TableClauses { get; set; } = new();

    public IEnumerable<CommonTableClause> GetCommonTableClauses()
    {
        foreach (var x in TableClauses) foreach (var item in x.GetCommonTableClauses()) yield return item;

        var lst = SubSelectClause?.GetAllWith().CommonTableAliases.ToList();
        if (SubSelectClause != null) foreach (var item in SubSelectClause.GetAllWith().CommonTableAliases) yield return item;
    }

    #region "AddJoin method"
    public TableClause Join(string tableName, string aliasName, RelationTypes type, Dictionary<string, string> keyvalues)
    {
        var d = new TableClause(tableName, aliasName);
        return Join(d, type, keyvalues);
    }

    public TableClause Join(SelectQuery subSelectClause, string aliasName, RelationTypes type, Dictionary<string, string> keyvalues)
    {
        var d = new TableClause(subSelectClause, aliasName);
        return Join(d, type, keyvalues);
    }

    public TableClause Join(TableClause destination, RelationTypes type, Dictionary<string, string> keyvalues)
    {
        destination.TableRelationClause = new TableRelationClause() { SourceName = AliasName, RelationType = type };
        keyvalues.ToList().ForEach(x => destination.TableRelationClause.Add(x.Key, x.Value));
        TableClauses.Add(destination);
        return destination;
    }

    public TableClause InnerJoin(string tableName, List<string> columns)
    {
        return Join(tableName, tableName, RelationTypes.Inner, columns.ToDictionary());
    }

    public TableClause InnerJoin(string tableName, string aliasName, List<string> columns)
    {
        return Join(tableName, aliasName, RelationTypes.Inner, columns.ToDictionary());
    }

    public TableClause InnerJoin(SelectQuery subSelectClause, string aliasName, List<string> columns)
    {
        return Join(subSelectClause, aliasName, RelationTypes.Inner, columns.ToDictionary());
    }

    public TableClause InnerJoin(TableClause destination, List<string> columns)
    {
        return Join(destination, RelationTypes.Inner, columns.ToDictionary());
    }

    public TableClause LeftJoin(string tableName, List<string> columns)
    {
        return Join(tableName, tableName, RelationTypes.Left, columns.ToDictionary());
    }

    public TableClause LeftJoin(string tableName, string aliasName, List<string> columns)
    {
        return Join(tableName, aliasName, RelationTypes.Left, columns.ToDictionary());
    }

    public TableClause LeftJoin(SelectQuery subSelectClause, string aliasName, List<string> columns)
    {
        return Join(subSelectClause, aliasName, RelationTypes.Left, columns.ToDictionary());
    }

    public TableClause LeftJoin(TableClause destination, List<string> columns)
    {
        return Join(destination, RelationTypes.Left, columns.ToDictionary());
    }

    public TableClause RightJoin(string tableName, List<string> columns)
    {
        return Join(tableName, tableName, RelationTypes.Right, columns.ToDictionary());

    }
    public TableClause RightJoin(string tableName, string aliasName, List<string> columns)
    {
        return Join(tableName, aliasName, RelationTypes.Right, columns.ToDictionary());
    }

    public TableClause RightJoin(SelectQuery subSelectClause, string aliasName, List<string> columns)
    {
        return Join(subSelectClause, aliasName, RelationTypes.Right, columns.ToDictionary());
    }

    public TableClause RightJoin(TableClause destination, List<string> columns)
    {
        return Join(destination, RelationTypes.Right, columns.ToDictionary());
    }

    public TableClause CrossJoin(string tableName)
    {
        return Join(tableName, tableName, RelationTypes.Cross, new());
    }

    public TableClause CrossJoin(string tableName, string aliasName)
    {
        return Join(tableName, aliasName, RelationTypes.Cross, new());
    }

    public TableClause CrossJoin(SelectQuery subSelectClause, string aliasName)
    {
        return Join(subSelectClause, aliasName, RelationTypes.Cross, new());
    }

    public TableClause CrossJoin(TableClause destination)
    {
        return Join(destination, RelationTypes.Cross, new());
    }
    #endregion

    public Query ToQuery()
    {
        var q = ToMyQuery();
        var nestQ = TableClauses.Select(x => x.ToQuery()).ToList();
        nestQ.ForEach(x => q = q.Merge(x, "\r\n"));
        return q;
    }

    private Query ToMyQuery()
    {
        var tableQ = ToFromQuery();

        if (TableRelationClause == null)
        {
            tableQ.CommandText = $"from {tableQ.CommandText}";
            return tableQ;
        }

        return TableRelationClause.ToQuery(AliasName, tableQ);
    }

    private Query ToFromQuery()
    {
        if (TableName != string.Empty) return ToTableQuery();
        return ToSubQuery();
    }

    private Query ToTableQuery()
    {
        if (TableName == string.Empty) throw new InvalidOperationException("TableName is required");
        if (AliasName == string.Empty) throw new InvalidOperationException("AliasName is required");

        if (TableName == AliasName) return new Query() { CommandText = TableName, Parameters = new() };

        var text = $"{TableName} as {AliasName}";
        return new Query() { CommandText = text, Parameters = new() };
    }

    private Query ToSubQuery()
    {
        if (AliasName == string.Empty) throw new InvalidOperationException("AliasName is required");
        if (SubSelectClause == null) throw new InvalidOperationException("SubSelectClause is required");

        var q = SubSelectClause.ToSubQuery();

        var text = (q.CommandText == AliasName) ? AliasName : $"{q.CommandText} as {AliasName}";
        return new Query() { CommandText = text, Parameters = q.Parameters };
    }
}