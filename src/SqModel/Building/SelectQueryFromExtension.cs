using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Building;

public static class SelectQueryFromExtension
{
    public static TableClause From(this SelectQuery source, CommonTableClause ct)
    {
        source.FromClause = new TableClause() { TableName = ct.AliasName, AliasName = ct.AliasName, RelationType = RelationTypes.From };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, string tableName)
    {
        source.FromClause = new TableClause() { TableName = tableName, AliasName = tableName, RelationType = RelationTypes.From };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, string tableName, string aliasName)
    {
        source.FromClause = new TableClause() { TableName = tableName, AliasName = aliasName, RelationType = RelationTypes.From };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, SelectQuery subquery, string alias)
    {
        source.FromClause = new TableClause() { SubSelectClause = subquery, AliasName = alias, RelationType = RelationTypes.From };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, Action<SelectQuery> action, string alias)
    {
        var q = new SelectQuery();
        action(q);
        source.FromClause = new TableClause() { SubSelectClause = q, AliasName = alias, RelationType = RelationTypes.From };
        return source.FromClause;
    }
}

