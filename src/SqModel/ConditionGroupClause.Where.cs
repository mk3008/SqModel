using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class ConditionGroupClauseWhere
{
    public static ConditionClause Where(this ConditionGroupClause source, TableClause table, string column, string parameterName, object parameterValue)
    {
        var c = new ConditionClause();
        c.CommandText = $"{table.AliasName}.{column} = {parameterName}";
        c.Parameters.Add(parameterName, parameterValue);

        source.ConditionClauses.Add(c);

        return c;
    }

    public static ConditionClause Where(this ConditionGroupClause source, string commandText)
    {
        var c = new ConditionClause();
        c.CommandText = commandText;

        source.ConditionClauses.Add(c);

        return c;
    }
}
