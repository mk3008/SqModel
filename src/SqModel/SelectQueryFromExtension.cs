using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQueryFromExtension
{
    public static TableClause From(this SelectQuery source, CommonTable ct)
    {
        source.FromClause = new TableClause() { TableName = ct.Name, AliasName = ct.Name, RelationType = RelationTypes.From };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, string tableName)
    {
        source.FromClause = new TableClause() { TableName = tableName, AliasName = tableName, RelationType = RelationTypes.From };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, SelectQuery subquery)
    {
        source.FromClause = new TableClause() { SubSelectClause = subquery, RelationType = RelationTypes.From };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, Action<SelectQuery> action)
    {
        var q = new SelectQuery();
        action(q);
        source.FromClause = new TableClause() { SubSelectClause = q, RelationType = RelationTypes.From };
        return source.FromClause;
    }
}

