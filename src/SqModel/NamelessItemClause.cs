using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel;
using SqModel.Extension;

namespace SqModel;

public class NamelessItemClause
{
    public NamelessItemClause(string token)
    {
        Token = token;
    }

    private string Token { get; init; }

    public List<NamelessItem> Collection { get; set; } = new();

    public bool IsOneLineFormat { get; set; } = true;

    public Query ToQuery()
    {
        if (!Collection.Any()) return new Query();

        if (IsOneLineFormat)
        {
            var q = Collection.Select(x => x.ToQuery()).ToList().ToQuery(", ");
            if (Token.IsNotEmpty()) q.CommandText = $"{Token} {q.CommandText}";
            return q;
        }
        else
        {
            var q = Collection.Select(x => x.ToQuery()).ToList().ToQuery("\r\n, ").InsertIndent();
            if (Token.IsNotEmpty()) q.CommandText = $"{Token}\r\n{q.CommandText}";
            return q;
        }
    }
}

public static class NamelessItemsExtension
{
    public static NamelessItem Add(this NamelessItemClause source)
    {
        var c = new NamelessItem();
        source.Collection.Add(c);
        return c;
    }
}