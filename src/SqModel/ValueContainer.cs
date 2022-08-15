using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ValueContainer
{
    public ValueClause Source { get; set; } = new();

    public ValueConjunction ValueConjunction { get; set; } = new();

    public Query ToQuery()
    {
        var sq = Source.ToQuery();
        var ds = ValueConjunction.ToQuery();
        sq = sq.Merge(ds, $" {ValueConjunction.Sign} ");
        return sq;
    }
}