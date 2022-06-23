using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQueryWith
{
    public static CommonTableClause With(this SelectQuery source, Action<SelectQuery> action, string alias)
    {
        var q = new SelectQuery();
        action(q);
        return source.With.Add(q, alias);
    }

    public static CommonTableClause With(this SelectQuery source, SelectQuery commonQuery, string alias)
    {
        return source.With.Add(commonQuery, alias);
    }
}
