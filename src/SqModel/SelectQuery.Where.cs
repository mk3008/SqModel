using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQueryWhere
{
    public static ConditionClause Where(this SelectQuery source, TableClause table, string column, string parameterName, object parameterValue)
    {
        var g = new ConditionGroupClause();
        source.WhereClause.ConditionGroupClauses.Add(g);

        return g.Where(table, column, parameterName, parameterValue);
    }

    public static ConditionClause Where(this SelectQuery source, string commandText)
    {
        var g = new ConditionGroupClause();
        source.WhereClause.ConditionGroupClauses.Add(g);

        return g.Where(commandText);
    }

    public static ConditionGroupClause WhereGroup(this SelectQuery source)
    {
        var g = new ConditionGroupClause();
        source.WhereClause.ConditionGroupClauses.Add(g);
        return g;
    }
}
