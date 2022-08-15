using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public WhereClause ParseWhereClause()
    {
        Logger?.Invoke($"ParseWhereClause start");

        var w = new WhereClause();
        var c = ParseOperatorContainer(); 
        c.IsRoot = true;
        w.Container = c;

        Logger?.Invoke($"ParseSingleTableClause end : {w.ToQuery().CommandText}");
        return w;
    }
}
