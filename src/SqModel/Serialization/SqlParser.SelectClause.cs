using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public SelectQuery ParseSelectQuery()
    {
        Logger?.Invoke($"ParseSelectQuery start");
        
        var q = new SelectQuery();
        var level = 1;

        foreach (var item in ReadTokens().Where(x => !x.StartsWith("--") && !x.StartsWith("/*")))
        {
            var token = item;
            Logger?.Invoke($"token : {token}");
            if (token == "(")
            {
                level++;
                continue;
            }

            if (token.ToLower() == "with")
            {
                q.With.CommonTableAliases.Add(ParseCommonTableClause());
                token = CurrentToken;

                while (token == ",")
                {
                    q.With.CommonTableAliases.Add(ParseCommonTableClause());
                    token = CurrentToken;
                }
            }

            if (token.ToLower() == "select")
            {
                q.SelectClause.Collection.Add(ParseValueClause());
                token = CurrentToken;

                while (token == ",")
                {
                    q.SelectClause.Collection.Add(ParseValueClause());
                    token = CurrentToken;
                }
            }

            if (token.ToLower() == "from")
            {
                q.FromClause = ParseTableClause(true);
                token = CurrentToken;
            }

            if (token.ToLower() == "where")
            {
                q.WhereClause = ParseWhereClause();
                continue;
            };

            if (token == ")")
            {
                level--;
                if (level == 0) break;
            }
        }

        Logger?.Invoke($"ParseSelectQuery end : {q.ToQuery().CommandText}");

        return q;
    }
}
