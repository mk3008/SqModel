using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel;
using SqModel.Extension;

namespace SqModel;

public class OrderClause
{
    public List<OrderItem> Collection { get; set; } = new();

    public bool IsOneLineFormat { get; set; } = true;

    public Query ToQuery()
    {
        if (!Collection.Any()) return new Query();

        if (IsOneLineFormat)
        {
            var q = Collection.Select(x => x.ToQuery()).ToList().ToQuery(", ");
            q.CommandText = $"order by {q.CommandText}";
            return q;
        }
        else
        {
            var q = Collection.Select(x => x.ToQuery()).ToList().ToQuery("\r\n, ").InsertIndent();
            q.CommandText = $"order by\r\n{q.CommandText}";
            return q;
        }
    }
}

public static class OrdertClauseExtension
{
    public static OrderItem Add(this OrderClause source)
    {
        var c = new OrderItem();
        source.Collection.Add(c);
        return c;
    }
}