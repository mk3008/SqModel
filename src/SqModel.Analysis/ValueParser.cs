//using SqModel.Analysis.Extensions;
//using SqModel.Core.Clauses;
//using SqModel.Core.Values;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;

//namespace SqModel.Analysis;

//public class ValueParser
//{
//    private static string[] BreakTokens { get; set; } = new string[]
//        {
//            ",",
//            "as",
//            "from",
//            "where",
//            "group by",
//            "having",
//            "order by",
//            "union",
//            "union all"
//        };

//    public static IValue Parse(TokenReader reader)
//    {
//        var value = ParseValueMain(reader);
//        var op = ParseOperatorOrDefault(reader);

//        while (!string.IsNullOrEmpty(op))
//        {
//            var v = ParseValueMain(reader);
//            value = value.AddOperatableValue(op, v);
//        }

//        return value;
//    }

//    private static ValueBase ParseValueMain(TokenReader reader)
//    {
//        var cache = new List<string>();
//        var firstToken = reader.ReadTokens(includeCurrentToken: true).FirstOrDefault();
//        if (firstToken == null) throw new NullReferenceException(nameof(firstToken));

//        if (firstToken.IsNumeric()) return ParseNumericValue(reader);
//        if (firstToken.IsOpenBracket()) return ParseBracketValue(reader);
//        if (firstToken.IsSingleQuote()) return ParseSingleQuoteTextValue(reader);

//        var c = reader.PeekOrDefault();
//        if (c == null) return new LiteralValue(firstToken);
//        if (c.IsDot()) return ParseColumnValue(reader);
//        if (c.IsOpenBracket()) return ParseFunctionValue(reader);

//        //case when


//        throw new NotSupportedException("");
//    }

//    private static ValueBase ParseColumnValue(TokenReader reader)
//    {
//        var cache = new List<string>();

//        foreach (var item in reader.ReadTokens(includeCurrentToken: true))
//        {
//            if (item.AreContains(BreakTokens) || item.IsOperator()) break;
//            if (cache.Count == 1 && !item.IsDot()) break;
//            if (cache.Count == 3) break;

//            cache.Add(item);
//        }
//        return new ColumnValue(cache[0], cache[2]);
//    }

//    private static ValueBase ParseNumericValue(TokenReader reader)
//    {
//        var cache = new List<string>();

//        foreach (var item in reader.ReadTokens(includeCurrentToken: true))
//        {
//            if (item.AreContains(BreakTokens) || item.IsOperator()) break;
//            if (cache.Count == 1 && !item.IsDot()) break;
//            if (cache.Count == 3) break;

//            cache.Add(item);
//        }
//        return new LiteralValue(cache.ToCommandText());
//    }

//    private static ValueBase ParseSingleQuoteTextValue(TokenReader reader)
//    {
//        var cache = new List<string>();

//        foreach (var item in reader.ReadTokens(includeCurrentToken: true))
//        {
//            if (item.AreContains(BreakTokens) || item.IsOperator()) break;
//            if (cache.Count == 3) break;
//            cache.Add(item);
//        }
//        if (cache.Count != 3) throw new SyntaxException("single quote is not closed.");

//        return new LiteralValue(cache.ToCommandText());
//    }

//    private static ValueBase ParseFunctionValue(TokenReader reader)
//    {
//        var cache = new List<string>();

//        foreach (var item in reader.ReadTokens(includeCurrentToken: true))
//        {
//            if (item.AreContains(BreakTokens) || item.IsOperator()) break;
//            if (cache.Count == 4) break;

//            cache.Add(item);
//        }
//        if (cache.Count != 4) throw new SyntaxException("bracket is not closed.");

//        return new FunctionValue(cache[0], cache[2]);
//    }

//    private static ValueBase ParseBracketValue(TokenReader reader)
//    {
//        var cache = new List<string>();

//        foreach (var item in reader.ReadTokens(includeCurrentToken: true))
//        {
//            if (item.AreContains(BreakTokens) || item.IsOperator()) break;
//            if (cache.Count == 3) break;

//            cache.Add(item);
//        }
//        if (cache.Count != 3) throw new SyntaxException("bracket is not closed.");

//        if (cache[1].IsSelectQuery())
//        {
//            //todo parse selectquery
//        }
//        return new LiteralValue(cache.ToCommandText());
//    }

//    private static string ParseOperatorOrDefault(TokenReader reader)
//    {
//        var item = reader.ReadTokens(includeCurrentToken: true).FirstOrDefault();
//        if (item == null || item.AreContains(BreakTokens)) return string.Empty;
//        if (item.IsOperator())
//        {
//            reader.ClearCache();
//            return item;
//        }
//        throw new SyntaxException($"near {item}");
//    }
//}