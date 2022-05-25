using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class SelectQuery
{
    public TableAlias Root { get; set; } = new();

    public List<CommonTableAlias> CommonTableAliases = new();

    public CommonTableAlias AddCommonTableAliases(string commandText, string alias, Dictionary<string, object>? prms = null)
    {
        var cta = new CommonTableAlias() { CommandText = commandText, AliasName = alias, Parameters = prms ?? new() };
        CommonTableAliases.Add(cta);
        return cta;
    }

    public CommonTableAlias AddCommonTableAlias(string commandText, TableAlias alias, Dictionary<string, object>? prms = null)
    {
        var cta = new CommonTableAlias() { CommandText = commandText, AliasName = alias.Table.TableName, Parameters = prms ?? new() };
        CommonTableAliases.Add(cta);
        return cta;
    }

    public List<TableRelation> TableRelations = new();

    public TableRelation AddTableRelation(TableAlias source, TableAlias destination, RelationTypes type = RelationTypes.Inner)
    {
        var tr = new TableRelation() { SourceTable = source, DestinationTable = destination, RelationType = type };
        TableRelations.Add(tr);
        return tr;
    }

    public List<Column> Columns { get; set; } = new();

    public Column AddColumn(TableAlias alias, string columnName, string? columnAlias = null)
    {
        var c = new Column() { TableName = alias.GetName(), ColumnName = columnName, AliasName = columnAlias ?? columnName };
        Columns.Add(c);
        return c;
    }

    private List<Column> GetSelectAllColumns()
    {
        var lst = new List<Column>();
        lst.Add(new Column() { TableName = Root.GetName(), ColumnName = "*" });
        TableRelations.ForEach(x => lst.Add(new Column() { TableName = x.DestinationTable.GetName(), ColumnName = "*" }));
        return lst;
    }

    public List<Query> GetColumnQueries()
    {
        var Qs = Columns.Select(x => x.ToQuery()).ToList();
        if (!Qs.Any())
        {
            GetSelectAllColumns().ForEach(x => Qs.Add(x.ToQuery()));
        }
        return Qs;
    }

    public Query ToQuery()
    {
        var rtQ = Root.ToQuery();
        var cQs = GetColumnQueries();
        var trQs = TableRelations.Select(x => x.ToQuery());

        //parameter
        var prms = new Dictionary<string, object>();
        prms.Merge(rtQ.Parameters);
        cQs.ToList().ForEach(x => prms.Merge(x.Parameters));
        trQs.ToList().ForEach(x => prms.Merge(x.Parameters));

        //command text
        var sb = new StringBuilder();
        if (CommonTableAliases.Any())
        {
            var ctaQs = CommonTableAliases.Select(x => x.ToQuery()).ToList();
            ctaQs.ToList().ForEach(x => prms.Merge(x.Parameters));

            sb.Append("with\r\n");
            sb.Append(ctaQs.Select(x => x.CommandText).ToString(",\r\n"));
            sb.Append("\r\n");
        }
        sb.Append($"select {cQs.Select(x => x.CommandText).ToString(", ")}\r\nfrom {rtQ.CommandText}");

        var relation = trQs.Select(x => x.CommandText).ToString("\r\n");
        if (relation != String.Empty) sb.Append($"\r\n{relation}");

        return new Query() { CommandText = sb.ToString(), Parameters = prms };
    }
}

public class CommonTableAlias
{
    public string CommandText { get; set; } = String.Empty;

    public Dictionary<string, object> Parameters { get; set; } = new();

    public string AliasName { get; set; } = string.Empty;

    public Query ToQuery()
    {
        /*
         * Alias as (
         *     CommandText
         * )
         */
        var sb = new StringBuilder();
        sb.Append($"{AliasName} as (\r\n    ");
        sb.Append(CommandText.Replace("\r\n", "\r\n    "));
        sb.Append($"\r\n)");

        return new Query() { CommandText = sb.ToString(), Parameters = Parameters };
    }
}

public class TableAlias
{
    public Table Table { get; set; } = new();

    public string AliasName { get; set; } = string.Empty;

    public string GetName() => (AliasName != String.Empty) ? AliasName : Table.GetName();

    public List<Column> GetColumns()
    {
        return Table.GetColumnNames().Select(x => new Column() { TableName = AliasName, ColumnName = x }).ToList();
    }

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

        var text = $"({tQ.CommandText}) as {GetName()}";
        return new Query() { CommandText = text, Parameters = tQ.Parameters };
    }
}

public class Table
{
    public Table() { }

    public Table(string tableName, string? aliasName = null)
    {
        TableName = tableName;
        AliasName = aliasName ?? tableName;
    }

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

    public Column AddColumn(string columnName, string? alias = null)
    {
        var c = new Column() { TableName = GetName(), ColumnName = columnName, AliasName = alias ?? columnName };
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

    public TableAlias ToTableAlias(string? alias = null)
    {
        return new TableAlias() { Table = this, AliasName = alias ?? GetName() };
    }
}

public class TableRelation
{
    public RelationTypes RelationType { get; set; } = RelationTypes.Inner;

    public TableAlias SourceTable { get; set; } = new();

    public TableAlias DestinationTable { get; set; } = new();

    public List<ColumnRelation> ColumnRelations { get; set; } = new();

    public TableRelation AddColumnRelation(RelatedColumn source, RelatedColumn destination, string sign = "=")
    {
        ColumnRelations.Add(new ColumnRelation() { SourceColumn = source, DestinationColumn = destination, Sign = sign });
        return this;
    }

    public TableRelation AddColumnRelation(string column)
    {
        AddColumnRelation(column, column);
        return this;
    }

    public TableRelation AddColumnRelation(string leftColumn, string rightColumn, string sign = "=")
    {
        var source = new RelatedColumn() { TableName = SourceTable.GetName(), ColumnName = leftColumn };
        var destination = new RelatedColumn() { TableName = DestinationTable.GetName(), ColumnName = rightColumn };

        ColumnRelations.Add(new ColumnRelation() { SourceColumn = source, DestinationColumn = destination, Sign = sign });
        return this;
    }

    public Query ToQuery()
    {
        var stQ = SourceTable.ToQuery();
        var dtQ = DestinationTable.ToQuery();
        var crsQs = ColumnRelations.Select(x => x.ToQuery());

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

public class ColumnRelation
{
    public RelatedColumn SourceColumn { get; set; } = new();

    public RelatedColumn DestinationColumn { get; set; } = new();

    public string Sign { get; set; } = "=";

    public Query ToQuery()
    {
        var lcQ = SourceColumn.ToQuery();
        var rcQ = DestinationColumn.ToQuery();

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

public class RelatedColumn
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