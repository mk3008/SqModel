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

    public Query ToQuery()
    {
        var tQ = Table.ToQuery();
        var text = $"({tQ.CommandText}) as {AliasName}";

        return new Query() { CommandText = text, Parameters = tQ.Parameters };
    }

    public List<Column> Columns => Table.Columns.Select(x => new Column() { TableName = AliasName, ColumnName = x.GetAliasName() }).ToList();
}

public class Table
{
    public string TableName { get; set; } = string.Empty;

    public string AliasName { get; set; } = string.Empty;

    public List<Column> Columns { get; set; } = new();

    public bool IsDistinct { get; set; } = false;

    private string GetTableName() => (AliasName != string.Empty) ? AliasName : TableName;

    public void AddColumn(string columnName)
    {
        Columns.Add(new Column() { TableName = GetTableName(), ColumnName = columnName });
    }

    public void AddColumn(string columnName, string aliasName)
    {
        Columns.Add(new Column() { TableName = GetTableName(), ColumnName = columnName, AliasName = aliasName });
    }

    public void AddCommandColumn(string commandText, string aliasName)
    {
        Columns.Add(new Column() { TableName = GetTableName(), CommandText = commandText, AliasName = aliasName });
    }

    public Query ToQuery()
    {
        var cQs = Columns.Select(x => x.ToQuery());

        var text = $"select {cQs.Select(x => x.CommandText).ToString(",")} from {TableName} as {GetTableName()}";

        var prms = new Dictionary<string, object>();
        cQs.ToList().ForEach(x => prms.Merge(x.Parameters));

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

    public string CommandText { get; set; } = string.Empty;

    public string AliasName { get; set; } = string.Empty;

    public Dictionary<string, object> Parameters { get; set; } = new();

    public string GetAliasName() => (AliasName != string.Empty) ? AliasName : ColumnName;

    public Query ToQuery()
    {
        if (CommandText != string.Empty && AliasName != string.Empty)
        {
            //ex. :val as value --:val = 1
            return new Query() { CommandText = $"{CommandText} as {GetAliasName()}", Parameters = Parameters };
        }
        if (TableName != string.Empty && ColumnName == "*")
        {
            //ex. tableA.*
            return new Query() { CommandText = $"{TableName}.{ColumnName}", Parameters = Parameters };
        }
        if (TableName != string.Empty && ColumnName != string.Empty)
        {
            //ex. tableA.ColumnX as X
            return new Query() { CommandText = $"{TableName}.{ColumnName} as {GetAliasName()}", Parameters = Parameters };
        }

        throw new NotSupportedException("ToQuery method failed.");
    }
}

public class ColumnRelation
{
    public string TableName { get; set; } = string.Empty;

    public string ColumnName { get; set; } = string.Empty;

    public string CommandText { get; set; } = string.Empty;

    public Dictionary<string, object> Parameters { get; set; } = new();

    public Query ToQuery()
    {
        if (CommandText != string.Empty)
        {
            //ex. :val --:val = 1
            return new Query() { CommandText = $"{CommandText}", Parameters = Parameters };
        }
        if (TableName != string.Empty && ColumnName != string.Empty)
        {
            //ex. tableA.ColumnX
            return new Query() { CommandText = $"{TableName}.{ColumnName}", Parameters = Parameters };
        }

        throw new NotSupportedException("ToQuery method failed.");
    }
}