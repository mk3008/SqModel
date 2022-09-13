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
            return parser.ParseCaseExpression();
        }

        var tmp = q.First();
        if (parser.ValueBreakTokens.Contains(tmp)) return new CommandValue() { CommandText = cache.ToString(" ") };
        cache.Add(tmp);

        if (parser.CurrentToken == "." && cache.Count == 2)
        {
            var item = new ColumnValue() { Table = cache.First(), Column = q.First() };
            q.First();
            return item;
        }

        tmp = q.First();
        while (tmp.IsNotEmpty() && !parser.ValueBreakTokens.Contains(tmp))
        {
            cache.Add(tmp);
            tmp = q.First();
        }

        return new CommandValue() { CommandText = cache.ToString(" ") };
    }
}
