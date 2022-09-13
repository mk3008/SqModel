using SqModel;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public IValueClause ParseValueClause()
    {
        Logger?.Invoke($"{nameof(ParseValueClause)} start");

        var cache = new List<string>();
        var q = ReadTokensWithoutComment();

        cache.Add(CurrentToken.IsNotEmpty() ? CurrentToken : q.First());

        if (CurrentToken == "(")
        {
            cache.Add(q.First()); // inner text

            using var p = new SqlParser(CurrentToken) { Logger = Logger };
            var f = p.ReadTokens().FirstOrDefault();
            if (f?.ToLower() == "select")
            {
                var iq = p.ParseSelectQuery();
                iq.IsOneLineFormat = true;
                var item = new SelectQueryValue() { Query = iq };
                q.First(); // skip inner text token
                if (CurrentToken != ")") throw new SyntaxException("Value clause syntax error");
                q.First(); // skip ')' text token
                return item;
            }
            cache.Add(q.First()); // cache ')' token
        }
        else if (CurrentToken.ToLower() == "case")
        {
            return ParseCaseExpression();
        }

        var tmp = q.First();
        if (ValueBreakTokens.Contains(tmp)) return new CommandValue() { CommandText = cache.ToString(" ") };
        cache.Add(tmp);

        if (CurrentToken == "." && cache.Count == 2)
        {
            var item = new ColumnValue() { Table = cache.First(), Column = q.First() };
            q.First();
            return item;
        }

        tmp = q.First();
        while (tmp.IsNotEmpty() && !ValueBreakTokens.Contains(tmp))
        {
            cache.Add(tmp);
            tmp = q.First();
        }

        return new CommandValue() { CommandText = cache.ToString(" ") };
    }
}
