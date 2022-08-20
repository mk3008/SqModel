using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
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

        var setRelationType = (string token) =>
        {
            var rel = token.ToRelationType();
            if (rel == RelationTypes.Undefined) throw new NotSupportedException();
            t.RelationType = rel;
        };

        var setSubquery = (string token) =>
        {
            using var p = new SqlParser(token);
            p.Logger = Logger;
            t.SubSelectClause = p.ParseSelectQuery();
            isSubQuery = false;
        };

        var setTableName = (string token) =>
        {
            t.TableName = token;
            isTable = false;
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

            if (ConditionBreakTokens.Any(token)) break;

            if (token.ToLower() == "as") continue;

            if (token.ToLower() == "on")
            {
                var c = ParseOperatorContainer();
                c.IsRoot = true;
                t.RelationConditionClause = c;
                if (string.IsNullOrEmpty(CurrentToken) || ConditionBreakTokens.Any(CurrentToken)) break;
                continue;
            }

            t.AliasName = token;
        }

        if (t.AliasName == String.Empty) t.AliasName = t.TableName;

        Logger?.Invoke($"ParseSingleTableClause end : {t.ToQuery().CommandText}");
        return t;
    }
}
