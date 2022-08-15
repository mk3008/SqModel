using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class WhereClause
{
    public OperatorContainer Container { get; set; } = new() { Splitter = "\r\n", IsRoot = true };

    public string Splitter { get; set; } = "\r\n";

    public Query ToQuery()
    {
        var q = Container.ToQuery();
        if (q.IsEmpty()) return q;
        q.CommandText = $"where{Splitter}{q.CommandText.InsertIndent()}";
        return q;
    }
}
