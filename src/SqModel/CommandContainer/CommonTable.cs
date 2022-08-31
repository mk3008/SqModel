using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Extension;

namespace SqModel.CommandContainer;

public class CommonTable : IQueryable
{
    public SelectQuery Query { get; set; } = new();

    public string Name { get; set; } = string.Empty;

    public Query ToQuery()
    {
        Query.IsincludeCte = false;
        var q = Query.ToQuery();
        q.CommandText = $"{Name} as (\r\n{q.CommandText.InsertIndent()}\r\n)";

        return q;
    }
}
