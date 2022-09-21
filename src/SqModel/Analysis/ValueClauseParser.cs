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
        var cache = new List<string>();
        var q = parser.ReadTokensWithoutComment();

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
        if (parser.CurrentToken.IsEmpty() || parser.ValueBreakTokens.Contains(parser.CurrentToken)) return ToCommandValue(cache);

        cache.Add(parser.CurrentToken);

        if (parser.CurrentToken == "." && cache.Count == 2)
        {
            var item = new ColumnValue() { Table = cache.First(), Column = q.First() };
            q.First();
            return item;
        }
        if (parser.CurrentToken == ".*" && cache.Count == 2)
        {
            var item = new ColumnValue() { Table = cache.First(), Column = "*" };
            q.First();
            return item;
        }

        cache.AddRange(ReadUntilValueBreak(parser));

        return ToCommandValue(cache);
    }

    private static List<string> ReadUntilValueBreak(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();
        var lst = new List<string>();

        var readBracket = () =>
        {
            q.First(); //inner text
            if (parser.CurrentToken.IsNotEmpty()) lst.Add(parser.CurrentToken);
            q.First();//close 
            lst.Add(parser.CurrentToken);
        };

        if (parser.CurrentToken == "(") readBracket();

        q.First();
        while (parser.CurrentToken.IsNotEmpty() && !parser.ValueBreakTokens.Contains(parser.CurrentToken))
        {
            if (parser.CurrentToken == "(")
            {
                lst.Add(parser.CurrentToken);
                readBracket();
            }
            else
            {
                lst.Add(parser.CurrentToken);
            }
            q.First();
        }
        return lst;
    }

    private static CommandValue ToCommandValue(List<string> cache)
    {
        var c = new CommandValue();
        var sb = new StringBuilder();
        var prev = string.Empty;
        cache.ForEach(x =>
        {
            if (x == "." || x == "(" || x == ")" || sb.Length == 0)
            {
                sb.Append(x);
            }
            else if (prev == "." || prev == "(")
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
