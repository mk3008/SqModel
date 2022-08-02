using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public ConditionClause ParseRelation()
    {
        Logger?.Invoke($"ParseRelation start");

        var r = new ConditionClause();

        r.Source = ParseValueClause();
        r.Sign = CurrentToken;
        r.Destination = ParseValueClause();

        Logger?.Invoke($"ParseRelation end : {r.ToQuery().CommandText}");
        return r;
    }
}
