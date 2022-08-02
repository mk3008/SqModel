using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ConditionClause
{
    public ValueClause Source { get; set; } = new();

    public ValueClause Destination { get; set; } = new();

    public string Sign { get; set; } = "=";

    public Query ToQuery()
    {
        var sq = Source.ToQuery();
        var ds = Destination.ToQuery();
        sq = sq.Merge(ds, $" {Sign} ");
        return sq;
    }
}