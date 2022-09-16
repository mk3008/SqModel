using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel;

namespace SqModel;

public class WithClause
{
    public List<CommonTable> Collection = new();

    public IEnumerable<CommonTable> GetCommonTableClauses()
    {
        foreach (var y in Collection)
        {
            if (y.Query == null) continue;
            foreach (var item in y.Query.GetAllWith().Collection) yield return item;
        }
        foreach (var item in Collection) yield return item;
    }

    internal CommonTable Add(SelectQuery selectQuery, string alias)
    {
        var c = new CommonTable() { Query = selectQuery, Name = alias };
        Collection.Add(c);
        return c;
    }

    public Query ToQuery()
    {
        var q = Collection.Select(x => x.ToQuery()).ToList().ToQuery(",\r\n");
        if (q.CommandText != string.Empty) q.CommandText = $"with\r\n{q.CommandText}";

        return q;
    }
}

public static class WithClauseExtension
{
    public static CommonTable Add(this WithClause source, Action<SelectQuery> action)
    {
        var sq = new SelectQuery();
        action(sq);
        var c = new CommonTable() { Query = sq };
        source.Collection.Add(c);
        return c;
    }
}