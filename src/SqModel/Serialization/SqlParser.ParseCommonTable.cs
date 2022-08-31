using SqModel.CommandContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public CommonTable ParseCommonTable(bool includeCurrentToken = false)
    {
        Logger?.Invoke($"{this.GetType().Name} start");

        var c = new CommonTable();
        var isSelect = false;
        foreach (var token in ReadTokensWithoutComment(includeCurrentToken))
        {
            Logger?.Invoke($"token : {token}");
            if (string.IsNullOrEmpty(c.Name))
            {
                c.Name = token;
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
                using var p = new SqlParser(token) { Logger = Logger };
                c.Query = p.ParseSelectQuery();
                isSelect = false;
                continue;
            }
            if (token == "select" || token == ",") break; //cte is end, or next common table.
        }

        Logger?.Invoke($"{this.GetType().Name} end : {c.ToQuery().CommandText}");
        return c;
    }
}
