using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.CommandContainer;

namespace SqModel.Clause;

public class WithClause
{
    public List<CommonTable> CommonTableAliases = new();

    public IEnumerable<CommonTable> GetCommonTableClauses()
    {
        foreach (var y in CommonTableAliases)
        {
            if (y.Query == null) continue;
            foreach (var item in y.Query.GetAllWith().CommonTableAliases) yield return item;
        }
        foreach (var item in CommonTableAliases) yield return item;
    }

    //public CommonTableClause Add(string commandText, string alias, Dictionary<string, object>? prms = null)
    //{
    //    var c = new CommonTableClause() { CommandText = commandText, AliasName = alias, Parameters = prms ?? new() };
    //    CommonTableAliases.Add(c);
    //    return c;
    //}

    internal CommonTable Add(SelectQuery selectQuery, string alias)
    {
        var c = new CommonTable() { Query = selectQuery, Name = alias };
        CommonTableAliases.Add(c);
        return c;
    }

    public Query ToQuery()
    {
        var q = CommonTableAliases.Select(x => x.ToQuery()).ToList().ToQuery(",\r\n");
        if (q.CommandText != string.Empty) q.CommandText = $"with\r\n{q.CommandText}";

        return q;
    }
}