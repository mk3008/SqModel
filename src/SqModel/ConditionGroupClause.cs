using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ConditionGroupClause
{
    public string LogicalOperator { get; set; } = "and";

    public List<ConditionClause> ConditionClauses { get; set; } = new();

    public List<ConditionGroupClause> ConditionGroupClauses { get; set; } = new();

    public Query ToQuery()
    {
        var q1 = new Query();
        ConditionClauses.ForEach(x => q1 = q1.Merge(x.ToQuery(), $" {LogicalOperator} "));
        if (ConditionClauses.Count > 1 && LogicalOperator.ToLower() != "and") q1.CommandText = $"({q1.CommandText})";

        ConditionGroupClauses.ForEach(x => q1 = q1.Merge(x.ToQuery(), $" {LogicalOperator} "));

        return q1;
    }
}
