//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SqModel;


//public class Column
//{
//    public string TableName { get; set; } = string.Empty;

//    public string ColumnName { get; set; } = string.Empty;

//    public string AliasName { get; set; } = string.Empty;

//    private Dictionary<string, object> Parameters { get; set; } = new();

//    public string GetName()
//    {
//        if (ColumnName == "*") return String.Empty;
//        return (AliasName != String.Empty) ? AliasName : ColumnName;
//    }

//    public Query ToQuery()
//    {
//        var name = GetName();

//        string? text;
//        if (name == string.Empty || name == ColumnName)
//        {
//            text = $"{TableName}.{ColumnName}";
//        }
//        else
//        {
//            text = $"{TableName}.{ColumnName} as {name}";
//        }

//        return new Query() { CommandText = text, Parameters = Parameters };
//    }
//}