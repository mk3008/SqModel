using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class UnionClause
{
    public bool IsUnionAll { get; set; } = true;

    public SelectQuery? SelectQuery { get; set; } = null;

    public Query ToQuery()
    {
        if (SelectQuery == null) return new Query();

        SelectQuery.IsIncludeCte = false;
        SelectQuery.IsIncludeOrder = false;

        var q = SelectQuery.ToQuery();
        if (IsUnionAll)
        {
            q = q.InsertToken("union all", "\r\n");
        }
        else
        {
            q = q.InsertToken("union", "\r\n");
        }
        return q;
    }
}
