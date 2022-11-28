//using SqModel.Core.Clauses;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SqModel.Analysis;

//public class AliasParser
//{
//    private static string[] BreakTokens { get; set; } = new string[]
//      {
//            ",",
//            "from",
//            "where",
//            "group by",
//            "having",
//            "order by",
//            "union",
//            "union all"
//      };

//    public static IValue Parse(TokenReader reader)
//    {
//        var value = ParseValue(reader);
//        var op = ParseOperatorOrDefault(reader);

//        while (!string.IsNullOrEmpty(op))
//        {
//            var v = ParseValue(reader);
//            value = value.AddOperatableValue(op, v);
//        }

//        return value;
//    }
//}
