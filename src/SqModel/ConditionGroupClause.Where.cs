using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class ConditionGroupClauseWhere
{
    public static ConditionClause WhereOr(this ConditionGroupClause source, string sourcevalue, string sign, string destinationvalue) => source.Where("or", sourcevalue, sign, destinationvalue);

    private static ConditionClause Where(this ConditionGroupClause source, string operatorToken, string sourcevalue, string sign, string destinationvalue)
    {
        var c = new ConditionClause();
        c.Operator = operatorToken;
        c.Source = new ValueClause() { Value = sourcevalue };
        c.Sign = sign;
        c.Destination = new ValueClause() { Value = destinationvalue };
        source.Conditions.Add(c);

        return c;
    }
}
