using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class WithClause
{
    public List<CommonTableClause> CommonTableAliases = new();

    public IEnumerable<CommonTableClause> GetCommonTableClauses()
    {
        foreach (var y in CommonTableAliases)
        {
            if (y.SelectQuery == null) continue;
            foreach (var item in y.SelectQuery.GetAllWith().CommonTableAliases) yield return item;
        }
        foreach (var item in CommonTableAliases) yield return item;
    }

    public CommonTableClause Add(string commandText, string alias, Dictionary<string, object>? prms = null)
    {
        var c = new CommonTableClause() { CommandText = commandText, AliasName = alias, Parameters = prms ?? new() };
        CommonTableAliases.Add(c);
        return c;
    }

    public CommonTableClause Add(SelectQuery selectQuery, string alias)
    {
        var c = new CommonTableClause() { SelectQuery = selectQuery, AliasName = alias };
        CommonTableAliases.Add(c);
        return c;
    }

    public Query ToQuery()
    {
        var q = CommonTableAliases.Select(x => x.ToQuery()).ToList().ToQuery(",\r\n");
        if (q.CommandText != String.Empty) q.CommandText = $"with\r\n{q.CommandText}";

        return q;
    }
}