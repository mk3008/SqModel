using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public ValueClause ParseValueClause(bool includeCurrentToken = false)
    {
        Logger?.Invoke($"ParseValueClause start");

        var c = new ValueClause();
        var cache = new List<string>();
        var isAlias = false;
        var isValue = false;
        var maybeIineQuery = false;
        var isInlineQuery = false;

        var foundBreakToken = () =>
        {
            if (cache.Count == 0 || c.Value != String.Empty || c.InlineQuery != null) return;

            // get alias
            if (cache.Count > 1 && cache.First() != "not" && cache.Last().IsLetter() || cache.Count > 2 && cache.First() == "not" && cache.Last().IsLetter())
            {
                c.AliasName = cache.Last();
                cache.RemoveAt(cache.Count - 1);
            }
            c.Value = cache.ToString(" ");
            cache.Clear();
        };

        var foundAsToken = () =>
        {
            if (c.Value == String.Empty && c.InlineQuery == null)
            {
                c.Value = cache.ToString(" ");
                cache.Clear();
            }
            isAlias = true;
        };

        var setAliasToken = (string t) =>
        {
            c.AliasName = t;
            isAlias = false;
        };

        var setTableToken = (string t) =>
        {
            c.TableName = cache.First();
            cache.Clear();
            isValue = true;
        };

        var setValueToken = (string t) =>
        {
            c.Value = t;
            isValue = false;
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

            using var p = new SqlParser(t) { Logger = Logger };
            c.InlineQuery = p.ParseSelectQuery();
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

            if (isValue)
            {
                setValueToken(token);
                continue;
            }

            if (!isInlineQuery) cache.Add(token);
            refreshInLineQueryFlag(token);
        }

        if (cache.Count != 0 && c.Value == String.Empty) c.Value = cache.ToString(" ");

        Logger?.Invoke($"ParseValueClause end : {c.ToQuery().CommandText}");
        return c;
    }
}
