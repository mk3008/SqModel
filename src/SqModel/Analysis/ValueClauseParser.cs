using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public static class ValueClauseParser
{
    public static IValueClause Parse(string text)
    {
        using var p = new SqlParser(text);
        return Parse(p);
    }

    public static IValueClause Parse(SqlParser parser)
    {
        var c = ParseCore(parser);
        if (!parser.CurrentToken.IsConjunction()) return c;

        var exp = new ValueExpression();
        exp.Collection.Add(c);

        while (parser.CurrentToken.IsConjunction())
        {
            var q = parser.ReadTokensWithoutComment();
            var con = parser.CurrentToken;
            q.First();

            var cmd = ValueClauseParser.ParseCore(parser);
            cmd.Conjunction = con;
            exp.Collection.Add(cmd);
        }

        return exp;
    }

    public static ValueExpression ParseAsExpression(SqlParser parser)
    {
        var c = ParseCore(parser);
        var exp = new ValueExpression();
        exp.Collection.Add(c);

        while (parser.CurrentToken.IsConjunction())
        {
            var q = parser.ReadTokensWithoutComment();
            var con = parser.CurrentToken;
            q.First();

            var cmd = ValueClauseParser.ParseCore(parser);
            cmd.Conjunction = con;
            exp.Collection.Add(cmd);
        }

        return exp;
    }

    private static IValueClause ParseCore(SqlParser parser)
    {
        var cache = new List<string>();
        var q = parser.ReadTokensWithoutComment();

        var recursiveParse = () =>
        {
            var text = Parse(parser).ToQuery().CommandText;
            cache.Add(text);
            return ToCommandValue(cache);
        };

        cache.Add(parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First());

        if (parser.CurrentToken == "(")
        {
            cache.Add(q.First()); // inner text

            using var p = new SqlParser(parser.CurrentToken) { Logger = parser.Logger };
            var f = p.ReadTokens().FirstOrDefault();
            if (f?.ToLower() == "select")
            {
                var iq = p.ParseSelectQuery();
                iq.IsOneLineFormat = true;
                var item = new SelectQueryValue() { Query = iq };
                q.First(); // skip inner text token
                if (parser.CurrentToken != ")") throw new SyntaxException("Value clause syntax error");
                q.First(); // skip ')' text token
                return item;
            }
            cache.Add(q.First()); // cache ')' token
        }
        else if (parser.CurrentToken.ToLower() == "case")
        {
            return CaseExpressionParser.Parse(parser);
        }

        q.First();
        if (parser.CurrentToken.IsEmpty() || parser.ValueBreakTokens.Contains(parser.CurrentToken.ToLower())) return ToCommandValue(cache);
        if (parser.CurrentToken.IsWord() && parser.CurrentToken.ToLower() != "is" && parser.CurrentToken.ToLower() != "null") return ToCommandValue(cache);
        if (parser.CurrentToken.IsConjunction()) return ToCommandValue(cache);

        cache.Add(parser.CurrentToken);

        if (parser.CurrentToken == "." && cache.Count == 2 && cache.First().IsLetter())
        {
            q.First();
            cache.Add(parser.CurrentToken);
            var col = parser.CurrentToken;
            q.First();
            if (parser.CurrentToken != "(")
            {
                var item = new ColumnValue() { Table = cache.First(), Column = col };
                return item;
            }
            cache.Add(parser.CurrentToken);
        }

        if (parser.CurrentToken == ".*" && cache.Count == 2)
        {
            var item = new ColumnValue() { Table = cache.First(), Column = "*" };
            q.First();
            return item;
        }

        if (parser.CurrentToken == "(")
        {
            q.First(); //inner text
            if (parser.CurrentToken.IsNotEmpty()) cache.Add(parser.CurrentToken);
            q.First(); //close 
            cache.Add(parser.CurrentToken);
        }

        q.First();

        if (parser.CurrentToken.ToLower() == "over" || parser.CurrentToken == "(")
        {
            return recursiveParse();
        }

        if (parser.CurrentToken.StartsWith("::"))
        {
            cache.Add(parser.CurrentToken);
            q.First();
        }

        return ToCommandValue(cache);
    }

    private static CommandValue ToCommandValue(List<string> cache)
    {
        var c = new CommandValue();
        var sb = new StringBuilder();
        var prev = string.Empty;
        cache.ForEach(x =>
        {
            if (sb.Length == 0)
            {
                sb.Append(x);
            }
            else if (prev.ToLower() == "exists")
            {
                sb.Append($" {x}");
            }
            else if (prev == "." || prev == "(")
            {
                sb.Append(x);
            }
            else if (x == "." || x.StartsWith("(") || x == ")" || x.StartsWith("::"))
            {
                sb.Append(x);
            }
            else
            {
                sb.Append($" {x}");
            }
            prev = x;
        });
        c.CommandText = sb.ToString();
        return c;
    }
}
