using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    private string[] ConditionBreakTokens =
        InnerJoinTokens
        .Union(LeftJoinTokens)
        .Union(RightJoinTokens)
        .Union(CrossJoinTokens)
        .Union(TableBreakTokens)
        .Union(QueryBreakTokens).ToArray();

    public static string[] FromTokens = new[]
    {
        "from",
    };

    public static string[] InnerJoinTokens = new[]
    {
        "inner join",
    };

    public static string[] LeftJoinTokens = new[]
    {
        "left join",
        "left outer join",
    };

    public static string[] RightJoinTokens = new[]
    {
        "right join",
        "right outer join",
    };

    public static string[] CrossJoinTokens = new[]
    {
        "cross join",
    };

    public static string[] TableBreakTokens = new[]
    {
        "where",
    };

    public static string[] QueryBreakTokens = new[]
    {
        ";",
    };

    public TableClause ParseTableClause(bool includeCurrentToken = false)
    {
        Logger?.Invoke($"ParseTableClause start");

        var t = ParseSingleTableClause(includeCurrentToken);

        while (CurrentToken.ToRelationType() != RelationTypes.Undefined)
        {
            var st = ParseSingleTableClause(true);
            t.SubTableClauses.Add(st);
        }

        Logger?.Invoke($"ParseTableClause end : {t.ToQuery().CommandText}");
        return t;
    }

    private TableClause ParseSingleTableClause(bool includeCurrentToken = false)
    {
        //
        Logger?.Invoke($"ParseSingleTableClause start");

        var t = new TableClause();
        var level = 1;
        var isTable = true;
        var isSubQuery = false;
        var isAlias = false;

        var setRelationType = (string token) =>
        {
            var rel = token.ToRelationType();
            if (rel == RelationTypes.Undefined) throw new NotSupportedException();
            t.RelationType = rel;
        };

        var setSubquery = (string token) =>
        {
            using var p = new Parser(token);
            p.Logger = Logger;
            t.SubSelectClause = p.ParseSelectQuery();
            isAlias = true;
        };

        var setTableName = (string token) =>
        {
            t.TableName = token;
            isTable = false;
            isAlias = true;
        };

        var setAlias = (string token) =>
        {
            t.AliasName = token;
            isAlias = false;
        };

        foreach (var tmp in ReadTokensWithoutComment(includeCurrentToken))
        {
            var token = tmp;
            Logger?.Invoke($"token : {token}");

            if (t.RelationType == RelationTypes.Undefined)
            {
                setRelationType(token);
                continue;
            }

            if (token == "(" && isTable)
            {
                level++;
                isTable = false;
                isSubQuery = true;
                continue;
            }

            if (token == ")")
            {
                level--;
                continue;
            }

            if (isSubQuery)
            {
                setSubquery(token);
                continue;
            }

            if (isTable)
            {
                setTableName(token);
                continue;
            }

            if (token.ToLower() == "as")
            {
                isAlias = true;
                continue;
            }

            if (isAlias)
            {
                setAlias(token);
                continue;
            }

            if (token.ToLower() == "on")
            {
                isAlias = false;
                t.RelationConditionClause = ParseConditionGroup();
                if (string.IsNullOrEmpty(CurrentToken) || ConditionBreakTokens.Any(CurrentToken)) break;
                continue;
            }

            if (ConditionBreakTokens.Any(token)) break;
        }

        if (t.AliasName == String.Empty) t.AliasName = t.TableName;

        Logger?.Invoke($"ParseSingleTableClause end : {t.ToQuery().CommandText}");
        return t;
    }
}
