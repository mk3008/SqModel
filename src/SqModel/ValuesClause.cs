using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ValuesClause
{
    public List<NamelessItemClause> Collection { get; set; } = new();

    public Query ToQuery()
    {
        /*
         * values
         *     (1, 2, 3)
         *     , (4, 5, 6)
         */
        var q = Collection.Select(x => x.ToQuery().DecorateBracket()).ToList().ToQuery("\r\n, ").InsertIndent();
        q.CommandText = $"values\r\n{q.CommandText}";
        return q;
    }
}
