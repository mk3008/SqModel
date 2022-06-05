//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SqModel;

//public class VirtualTable
//{

//    public string TableName { get; set; } = string.Empty;

//    public string AliasName { get; set; } = string.Empty;

//    public SelectColumnClause ColumnClause { get; set; } = new();//ColumnList

//    private Column GetSelectAllColumn() => new Column() { TableName = GetName(), ColumnName = "*" };

//    public List<string> GetColumnNames()
//    {
//        var lst = ColumnClause.GetColumnNames();
//        if (!lst.Any()) lst.Add(GetSelectAllColumn().ColumnName);
//        return lst;
//    }

//    public Query GetColumnQuery()
//    {
//        var q = ColumnClause.ToQuery();
//        if (q.CommandText != String.Empty) return q;
//        return GetSelectAllColumn().ToQuery();
//    }

//    public bool IsDistinct { get; set; } = false;

//    public string GetName() => (AliasName != string.Empty) ? AliasName : TableName;

//    //public Column AddColumn(string columnName, string? alias = null)
//    //{
//    //    var c = new Column() { TableName = GetName(), ColumnName = columnName, AliasName = alias ?? columnName };
//    //    ColumnClause.Add(c);
//    //    return c;
//    //}

//    //public VirtualColumn AddVirtualColumn(string commandText, string aliasName)
//    //{
//    //    var c = new VirtualColumn() { CommandText = commandText, AliasName = aliasName };
//    //    VirtualColumnClause.Add(c);
//    //    return c;
//    //}

//    public Query ToQuery()
//    {
//        var name = GetName();
//        var tbl = (name == TableName) ? TableName : $"{TableName} as {name}";

//        var q = GetColumnQuery();

//        var text = $"select {q.CommandText} from {tbl}";

//        return new Query() { CommandText = text, Parameters = q.Parameters };
//    }

//    public SelectClause ToTableAlias(string? alias = null)
//    {
//        return new SelectClause() { Table = this, AliasName = alias ?? GetName() };
//    }
//}
