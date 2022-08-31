using SqModel.Command;
using SqModel.CommandContainer;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public SelectItem ParseSelectItem(bool includeCurrentToken = false)
    {
        Logger?.Invoke($"{nameof(ParseSelectItem)} start");

        var item = new SelectItem();
        var cache = new List<string>();

        var isAlias = false;

        var isValue = false;
        var table = string.Empty;
        var value = string.Empty;

        var maybeIineQuery = false;
        var isInlineQuery = false;

        var foundBreakToken = () =>
        {
            if (cache.Count == 0 || item.Command != null) return;

            // get alias
            if (cache.Count > 1 && cache.First() != "not" && cache.Last().IsLetter() || cache.Count > 2 && cache.First() == "not" && cache.Last().IsLetter())
            {
                item.As(cache.Last());
                cache.RemoveAt(cache.Count - 1);
            }
            item.Command = new CommandValue() { CommandText = cache.ToString(" ") };
            cache.Clear();
        };

        var foundAsToken = () =>
        {
            if (item.Command == null)
            {
                item.Command = new CommandValue() { CommandText = cache.ToString(" ") };
                cache.Clear();
            }
            isAlias = true;
        };

        var setAliasToken = (string t) =>
        {
            if (item == null) throw new SyntaxException("alias error.");
            item.As(t);
        };

        var setTableToken = (string t) =>
        {
            if (table.IsNotEmpty()) throw new SyntaxException("table error.");
            table = cache.First();
            cache.Clear();
            isValue = true;
        };

        var setValueToken = (string t) =>
        {
            if (item.Command != null) throw new SyntaxException("command error.");
            if (table.IsEmpty())
            {
                item.Command = new CommandValue() { CommandText = t };
            }
            else
            {
                item.Command = new ColumnCommand() { Table = table, Column = t };
            }
            isAlias = true;
        };

        var setCaseExpression = () =>
        {
            if (item.Command != null) throw new SyntaxException("command error.");
            item.Command = ParseCaseExpression(true);
            isAlias = true;
        };

        var tryParseInlineQuery = (string t) =>
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
                return;
            }

            if (item.Command != null) throw new SyntaxException("command error.");
            using var p = new SqlParser(t) { Logger = Logger };
            item.Command = new SelectQueryCommand() { Query = p.ParseSelectQuery() };
            cache.Clear();
        };

        var refreshInLineQueryFlag = (string t) => maybeIineQuery = (t == "(" && cache.Count == 1) ? true : false;

        foreach (var token in ReadTokensWithoutComment(includeCurrentToken))
        {
            Logger?.Invoke($"token : {token}");

            if (ValueBreakTokens.Where(x => x == token.ToLower()).Any())
            {
                foundBreakToken();
                break;
            }

            if (token.ToLower() == "as")
            {
                foundAsToken();
                continue;
            }

            if (isAlias)
            {
                setAliasToken(token);
                continue;
            }

            if (maybeIineQuery)
            {
                tryParseInlineQuery(token);
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
                setCaseExpression();
                continue;
            }

            if (isValue)
            {
                setValueToken(token);
                continue;
            }

            if (!isInlineQuery) cache.Add(token);
            refreshInLineQueryFlag(token);
        }

        if (cache.Count != 0 && item.Command == null) item.Command = new CommandValue() { CommandText = cache.ToString(" ") };

        Logger?.Invoke($"{nameof(ParseSelectItem)} end : {item.ToQuery().CommandText}");
        return item;
    }
}
