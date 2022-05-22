using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class SelectQuery
{
    public TableAlias RootTable { get; set; } = new();

    public List<TableRelation> TableRelations = new();

    public List<Column> Columns { get; set; } = new();

    public Query ToQuery()
    {
        var rtQ = RootTable.ToQuery();
        var cQs = Columns.Select(x => x.ToQuery());
        var trQs = TableRelations.Select(x => x.ToQuery());

        //command text
        var text = $"select {cQs.Select(x => x.CommandText).ToString(",")}\r\nfrom {rtQ.CommandText}";
        var relation = trQs.Select(x => x.CommandText).ToString("\r\n");
        if (relation != String.Empty) text += $"\r\n{relation}";

        //parameter
        var prms = new Dictionary<string, object>();
        prms.Merge(rtQ.Parameters);
        cQs.ToList().ForEach(x => prms.Merge(x.Parameters));
        trQs.ToList().ForEach(x => prms.Merge(x.Parameters));

        return new Query() { CommandText = text, Parameters = prms };
    }
}

public class TableAlias
{
    public Table Table { get; set; } = new();

    public string AliasName { get; set; } = string.Empty;

    public string GetName() => (AliasName != String.Empty) ? AliasName : Table.GetName();

    public Query ToQuery()
    {
        var tQ = Table.ToQuery();

        var names = Table.GetColumnNames();
        if (names.Count == 1 && names[0] == "*")
        {
            var name = GetName();
            if (Table.TableName == name)
            {
                return new Query() { CommandText = Table.TableName, Parameters = tQ.Parameters };
            }
            return new Query() { CommandText = $"{Table.TableName} as {name}", Parameters = tQ.Parameters };
        }

        var text = $"({tQ.CommandText}) as {AliasName}";
        return new Query() { CommandText = text, Parameters = tQ.Parameters };
    }

    public List<Column> GetColumns()
    {
        return Table.GetColumnNames().Select(x => new Column() { TableName = AliasName, ColumnName = x }).ToList();
    }
}

public class Table
{
    public string TableName { get; set; } = string.Empty;

    public string AliasName { get; set; } = string.Empty;

    public List<Column> Columns { get; set; } = new();

    public List<VirtualColumn> VirtualColumns { get; set; } = new();

    private Column GetSelectAllColumn() => new Column() { TableName = GetName(), ColumnName = "*" };

    public List<string> GetColumnNames()
    {
        var lst = new List<string>();
        Columns.ForEach(x => lst.Add(x.GetName()));
        if (!lst.Any()) lst.Add(GetSelectAllColumn().ColumnName);
        VirtualColumns.ForEach(x => lst.Add(x.AliasName));
        return lst;
    }

    public List<Query> GetColumnQueries()
    {
        var Qs = Columns.Select(x => x.ToQuery()).ToList();
        if (!Qs.Any())
        {
            Qs.Add(GetSelectAllColumn().ToQuery());
        }
        Qs.AddRange(VirtualColumns.Select(x => x.ToQuery()));
        return Qs;
    }

    public bool IsDistinct { get; set; } = false;

    public string GetName() => (AliasName != string.Empty) ? AliasName : TableName;

    public Column AddColumn(string columnName)
    {
        var c = new Column() { TableName = GetName(), ColumnName = columnName };
        Columns.Add(c);
        return c;
    }

    public Column AddColumn(string columnName, string alias)
    {
        var c = new Column() { TableName = GetName(), ColumnName = columnName, AliasName = alias };
        Columns.Add(c);
        return c;
    }

    public VirtualColumn AddVirtualColumn(string commandText, string aliasName)
    {
        var c = new VirtualColumn() { CommandText = commandText, AliasName = aliasName };
        VirtualColumns.Add(c);
        return c;
    }

    public Query ToQuery()
    {
        var name = GetName();
        var tbl = (name == TableName) ? TableName : $"{TableName} as {name}";

        var Qs = GetColumnQueries();

        var text = $"select {Qs.Select(x => x.CommandText).ToString(", ")} from {tbl}";

        var prms = new Dictionary<string, object>();
        Qs.ForEach(x => prms.Merge(x.Parameters));

        return new Query() { CommandText = text, Parameters = prms };
    }
}

public class TableRelation
{
    public RelationTypes RelationType { get; set; } = RelationTypes.Inner;

    public TableAlias SourceTable { get; set; } = new();

    public TableAlias DestinationTable { get; set; } = new();

    public List<ColumnRelationSet> ColumnRelationSets { get; set; } = new();

    public Query ToQuery()
    {
        var stQ = SourceTable.ToQuery();
        var dtQ = DestinationTable.ToQuery();
        var crsQs = ColumnRelationSets.Select(x => x.ToQuery());

        //parameter
        var prms = new Dictionary<string, object>();
        prms.Merge(stQ.Parameters);
        prms.Merge(dtQ.Parameters);
        crsQs.ToList().ForEach(x => prms.Merge(x.Parameters));

        if (RelationType == RelationTypes.Cross)
        {
            return new Query() { CommandText = $"cross join {dtQ.CommandText}", Parameters = prms };
        }

        //command text
        var condition = crsQs.Select(x => x.CommandText).ToString(" and ");

        var join = string.Empty;
        switch (RelationType)
        {
            case RelationTypes.Inner:
                join = "inner join";
                break;
            case RelationTypes.Left:
                join = "left join";
                break;
            case RelationTypes.Right:
                join = "right join";
                break;
        }

        var text = $"{join} {dtQ.CommandText} on {condition}";

        return new Query() { CommandText = text, Parameters = prms };
    }
}
public enum RelationTypes
{
    Inner = 0,
    Left = 1,
    Right = 2,
    Cross = 3,
}

public class ColumnRelationSet
{
    public ColumnRelation LeftColumn { get; set; } = new();

    public ColumnRelation RightColumn { get; set; } = new();

    public string Sign { get; set; } = "=";

    public Query ToQuery()
    {
        var lcQ = LeftColumn.ToQuery();
        var rcQ = RightColumn.ToQuery();

        var text = $"{lcQ.CommandText} {Sign} {rcQ.CommandText}";

        var prms = new Dictionary<string, object>();
        prms.Merge(lcQ.Parameters);
        prms.Merge(rcQ.Parameters);

        return new Query() { CommandText = text, Parameters = prms };
    }
}
public class Column
{
    public string TableName { get; set; } = string.Empty;

    public string ColumnName { get; set; } = string.Empty;

    public string AliasName { get; set; } = string.Empty;

    private Dictionary<string, object> Parameters { get; set; } = new();

    public string GetName()
    {
        if (ColumnName == "*") return String.Empty;
        return (AliasName != String.Empty) ? AliasName : ColumnName;
    }

    public Query ToQuery()
    {
        var name = GetName();

        string? text;
        if (name == string.Empty || name == ColumnName)
        {
            text = $"{TableName}.{ColumnName}";
        }
        else
        {
            text = $"{TableName}.{ColumnName} as {name}";
        }

        return new Query() { CommandText = text, Parameters = Parameters };
    }
}

public class VirtualColumn
{
    public string CommandText { get; set; } = string.Empty;

    public string AliasName { get; set; } = string.Empty;

    public Dictionary<string, object> Parameters { get; set; } = new();

    public VirtualColumn AddParameter(string key, object value)
    {
        Parameters[key] = value;
        return this;
    }

    public Query ToQuery()
    {
        if (CommandText != string.Empty && AliasName != string.Empty)
        {
            //ex. :val as value --:val = 1
            return new Query() { CommandText = $"{CommandText} as {AliasName}", Parameters = Parameters };
        }

        throw new NotSupportedException("ToQuery method failed.");
    }
}

public class ColumnRelation
{
    public string TableName { get; set; } = string.Empty;

    public string ColumnName { get; set; } = string.Empty;

    private Dictionary<string, object> Parameters { get; set; } = new();

    public Query ToQuery()
    {
        if (TableName != string.Empty && ColumnName != string.Empty)
        {
            //ex. tableA.ColumnX
            return new Query() { CommandText = $"{TableName}.{ColumnName}", Parameters = Parameters };
        }

        throw new NotSupportedException("ToQuery method failed.");
    }
}