using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class JoinColumnRelationClause
{
    public List<ColumnRelationClause> ColumnRelationClauses { get; set; } = new();

    public Query ToQuery()
    {
        return ColumnRelationClauses.Select(x => x.ToQuery()).ToList().ToQuery(" and ");
    }
}
