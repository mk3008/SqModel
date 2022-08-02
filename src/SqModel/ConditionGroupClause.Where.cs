﻿using System;
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
        c.Source = new ValueClause() { TableName = table.TableName , Value = column};
        c.Sign = "=";
        c.Destination = new ValueClause() { Value = parameterName }; 
        c.Destination.Parameters.Add(parameterName, parameterValue);

        source.ConditionClauses.Add(c);

        return c;
    }

    public static ConditionClause Where(this ConditionGroupClause source, string sourcevalue, string sign, string destinationvalue)
    {
        var c = new ConditionClause();
        c.Source = new ValueClause() { Value = sourcevalue };
        c.Sign = sign;
        c.Destination = new ValueClause() { Value = destinationvalue };

        source.ConditionClauses.Add(c);

        return c;
    }
}
