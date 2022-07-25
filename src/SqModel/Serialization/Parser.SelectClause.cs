using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public SelectQuery ParseSelectQuery()
    {
        var q = new SelectQuery();
        //var isWith = false;
        //var isSelect = false;
        var level = 1;

        Logger?.Invoke($"ParseSelectQuery start");

        foreach (var item in ReadAllTokens().Where(x => !x.StartsWith("--") && !x.StartsWith("/*")))
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
                //isWith = true;
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
                //isWith = false;
                //isSelect = true;

                q.SelectClause.ColumnClauses.Add(ParseColumnClause());
                token = CurrentToken;

                while (token == ",")
                {
                    q.SelectClause.ColumnClauses.Add(ParseColumnClause());
                    token = CurrentToken;
                }
            }

            if (token.ToLower() == "from")
            {
                //isSelect = false;
                q.FromClause = ParseTableClause();
                token = CurrentToken;
            }

            if (token.ToLower() == "where")
            {
                //q.WhereClause = null;
                continue;
            };

            if (token == ")")
            {
                level--;
                if (level == 0) break;
            }
        }

        Logger?.Invoke($"ParseSelectQuery end");

        return q;
    }
}
