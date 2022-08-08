using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public ConditionClause ParseCondition(bool includeCurrentToken= false)
    {
        Logger?.Invoke($"ParseCondition start");

        var c = new ConditionClause();

        c.Source = ParseValueClause(includeCurrentToken);
        c.Sign = CurrentToken;
        c.Destination = ParseValueClause();

        Logger?.Invoke($"ParseCondition end : {c.ToQuery().CommandText}");
        return c;
    }
}
