using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public ColumnClause ParseColumnClause()
    {
        Logger?.Invoke($"ParseColumnClause start");

        var c = new ColumnClause();
        var cache = new List<string>();
        var isAlias = false;
        var isColumn = false;
        foreach (var token in ReadAllTokens().Where(x => !x.StartsWith("--") && !x.StartsWith("/*")))
        {
            Logger?.Invoke($"token : {token}");

            if (token == "," || token.ToLower() == "from")
            {
                if (cache.Count == 0 || c.ColumnName != String.Empty) break;

                // column name only pattern
                if (cache.Count == 1)
                {
                    c.ColumnName = cache.First();
                    cache.Clear();
                    break;
                }

                // get alias
                if (cache.Last().IsLetter())
                {
                    c.AliasName = cache.Last();
                    cache.RemoveAt(cache.Count - 1);
                }

                c.ColumnName = cache.ToString(" ");
                cache.Clear();
                break;
            }

            if (token.ToLower() == "as")
            {
                // omit table name pattern
                if (c.ColumnName == string.Empty && cache.Count > 0)
                {
                    c.ColumnName = cache.ToString(" ");
                    cache.Clear();
                }
                isAlias = true;
                continue;
            }

            if (isAlias)
            {
                c.AliasName = token;
                isAlias = false;
                continue;
            }

            if (token == "." && cache.Count == 1)
            {
                c.TableName = cache.First();
                cache.Clear();
                isColumn = true;
                continue;
            }

            if (isColumn)
            {
                c.ColumnName = token;
                isColumn = false;
                isAlias = true;
                continue;
            }

            cache.Add(token);
        }

        Logger?.Invoke($"ParseColumnClause end : {c.ToQuery().CommandText}");
        return c;
    }
}
