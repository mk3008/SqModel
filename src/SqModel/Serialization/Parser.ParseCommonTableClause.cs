using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public CommonTableClause ParseCommonTableClause()
    {
        Logger?.Invoke($"ParseCommonTableClause start");

        var c = new CommonTableClause();
        var isSelect = false;
        foreach (var token in ReadTokens().Where(x => !x.StartsWith("--") && !x.StartsWith("/*")))
        {
            Logger?.Invoke($"token : {token}");
            if (string.IsNullOrEmpty(c.AliasName))
            {
                c.AliasName = token;
                continue;
            }
            if (token == "as" || token == ")") continue;
            if (token == "(")
            {
                isSelect = true;
                continue;
            }
            if (isSelect)
            {
                //token is select query string.
                using var p = new Parser(token);
                p.Logger = Logger;
                c.SelectQuery = p.ParseSelectQuery();
                isSelect = false;
                continue;
            }
            if (token == "select" || token == ",") break;
        }

        Logger?.Invoke($"ParseCommonTableClause end : {c.ToQuery().CommandText}");
        return c;
    }
}
