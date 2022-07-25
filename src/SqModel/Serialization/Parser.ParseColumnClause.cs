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
        var cache = string.Empty;

        foreach (var token in ReadAllTokens().Where(x => !x.StartsWith("--") && !x.StartsWith("/*")))
        {
            Logger?.Invoke($"token : {token}");

            if (token == "(" || token == ")" || token == "as") continue;
            if (string.IsNullOrEmpty(cache))
            {
                cache = token;
                continue;
            }
            if (token == ".")
            {
                c.TableName = cache;
                continue;
            }
            if (token == "," || token.ToLower() == "from")
            {
                // column name only pattern
                if (c.ColumnName == String.Empty) c.ColumnName = cache;
                break;
            }
            if (c.TableName == String.Empty && cache != String.Empty && c.ColumnName == String.Empty)
            {
                // omit table name and has alias name pattern
                c.ColumnName = cache;
                c.AliasName = token;
                continue;
            }
            if (string.IsNullOrEmpty(c.ColumnName))
            {
                c.ColumnName = token;
                continue;
            }
            if (string.IsNullOrEmpty(c.AliasName))
            {
                c.AliasName = token;
                continue;
            }
        }

        Logger?.Invoke($"ParseColumnClause end : {c.ToQuery().CommandText}");
        return c;
    }
}
