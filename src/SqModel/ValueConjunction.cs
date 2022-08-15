using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ValueConjunction
{
    public string Sign { get; set; } = "=";

    public ValueClause Destination { get; set; } = new();

    public Query ToQuery()
        => Destination.ToQuery();
}
