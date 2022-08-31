using SqModel.CommandContainer;
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

        foreach (var item in ReadTokensWithoutComment(false))
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
                q.With.CommonTableAliases.Add(ParseCommonTable());
                token = CurrentToken;

                while (token == ",")
                {
                    q.With.CommonTableAliases.Add(ParseCommonTable());
                    token = CurrentToken;
                }
            }

            if (token.ToLower() == "select")
            {
                var fn = () =>
                {
                    var c = new SelectItem() { Command = ParseValueClause() };
                    q.SelectClause.Collection.Add(c);

                    token = CurrentToken;
                    if (token == "as")
                    {
                        c.As(ReadTokensWithoutComment(false).First());
                    }
                    //else if (token != "from" && token != ",")
                    //{
                    //    c.As(token);
                    //    token = ReadTokensWithoutComment(false).First();
                    //}
                };

                fn();
                while (token == ",") fn();
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
