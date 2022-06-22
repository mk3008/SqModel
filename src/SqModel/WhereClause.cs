using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class WhereClause
{
    public List<ConditionGroupClause> ConditionGroupClauses { get; set; } = new();

    public Query ToQuery()
    {
        var q = ConditionGroupClauses.Select(c => c.ToQuery()).ToList().ToQuery("\r\nand ");
        if (ConditionGroupClauses.Any()) q.CommandText = $"where\r\n{q.CommandText.Indent()}";
        return q;
    }
}
