using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class WhereClause
{
    public ConditionGroupClause ConditionClause { get; set; } = new() { Splitter = "\r\n"};

    public Query ToQuery()
    {
        var q = ConditionClause.ToQuery();
        if (q.CommandText != String.Empty) q.CommandText = $"where\r\n{q.CommandText.Indent()}";
        return q;
    }
}
