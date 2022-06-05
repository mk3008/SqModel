//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SqModel;

//public class VirtualColumn
//{
//    public string CommandText { get; set; } = string.Empty;

//    public string AliasName { get; set; } = string.Empty;

//    public Dictionary<string, object> Parameters { get; set; } = new();

//    public VirtualColumn AddParameter(string key, object value)
//    {
//        Parameters[key] = value;
//        return this;
//    }

//    public Query ToQuery()
//    {
//        if (CommandText != string.Empty && AliasName != string.Empty)
//        {
//            //ex. :val as value --:val = 1
//            return new Query() { CommandText = $"{CommandText} as {AliasName}", Parameters = Parameters };
//        }

//        throw new NotSupportedException("ToQuery method failed.");
//    }
//}