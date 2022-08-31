using SqModel.Command;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public ICommand ParseValueClause(bool includeCurrentToken = false)
    {
        Logger?.Invoke($"{nameof(ParseValueClause)} start");

        ICommand? c = null;
        var cache = new List<string>();

        var isValue = false;
        var table = string.Empty;
        var value = string.Empty;

        var maybeIineQuery = false;
        var isInlineQuery = false;

        var setTableToken = (string t) =>
        {
            table = cache.First();
            cache.Clear();
            isValue = true;
        };

        Func<ICommand?> createCommandOrDefault = () =>
        {
            if (cache.Count == 0 || c != null) return null;
            return new CommandValue() { CommandText = cache.ToString(" ") };
        };

        Func<string, ICommand> createCommandValue = t =>
        {
            if (table.IsEmpty())
            {
                return new CommandValue() { CommandText = t };
            }
            else
            {
                return new ColumnCommand() { Table = table, Column = t };
            }
        };

        Func<string, ICommand?> parseInlineQueryOrDefault = t =>
        {
            maybeIineQuery = false;

            using (var parser = new SqlParser(t) { Logger = Logger })
            {
                var f = parser.ReadTokens().FirstOrDefault();
                if (f?.ToLower() == "select") isInlineQuery = true;
            }

            if (!isInlineQuery)
            {
                cache.Add(t);
                return null; ;
            }

            using var p = new SqlParser(t) { Logger = Logger };
            var inq = p.ParseSelectQuery();
            inq.IsOneLineFormat = true;
            return new SelectQueryCommand() { Query = inq };
        };

        var refreshInLineQueryFlag = (string t) => maybeIineQuery = (t == "(" && cache.Count == 1) ? true : false;

        foreach (var token in ReadTokensWithoutComment(includeCurrentToken))
        {
            Logger?.Invoke($"token : {token}");

            if (ValueBreakTokens.Where(x => x == token.ToLower()).Any()) break;

            if (maybeIineQuery)
            {
                c = parseInlineQueryOrDefault(token);
                if (c != null) break;
                continue;
            }

            if (isInlineQuery && token == ")") continue;

            if (token == "." && cache.Count == 1)
            {
                setTableToken(token);
                continue;
            }

            if (token.ToLower() == "case")
            {
                c = ParseCaseExpression(true);
                break;
            }

            if (isValue)
            {
                c = createCommandValue(token);
                break;
            }

            if (!isInlineQuery) cache.Add(token);
            refreshInLineQueryFlag(token);
        }

        if (c == null) c = createCommandOrDefault();
        if (c == null) throw new SyntaxException("command");

        Logger?.Invoke($"{nameof(ParseValueClause)} end : {c.ToQuery().CommandText}");

        return c;
    }
}
