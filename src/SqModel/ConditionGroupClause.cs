using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ConditionGroupClause
{
    public string LogincalOperator { get; set; } = "or";

    public List<ConditionClause> ConditionClauses { get; set; } = new();

    public Query ToQuery()
    {
        var q = ConditionClauses.Select(c => c.ToQuery()).ToList().ToQuery($" {LogincalOperator} ");
        if (ConditionClauses.Count > 1) q.CommandText = $"({q.CommandText})";

        return q;
    }
}
