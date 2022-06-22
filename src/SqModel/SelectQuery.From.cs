using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQueryFrom
{
    public static TableClause From(this SelectQuery source, string tableName)
    {
        source.FromClause = new TableClause() { TableName = tableName, AliasName = tableName };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, string tableName, string aliasName)
    {
        source.FromClause = new TableClause() { TableName = tableName, AliasName = aliasName };
        return source.FromClause;
    }

    public static TableClause From(this SelectQuery source, SelectQuery subquery, string alias)
    {
        source.FromClause = new TableClause() { SubSelectClause = subquery, AliasName = alias };
        return source.FromClause;
    }
}

