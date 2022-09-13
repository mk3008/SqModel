﻿using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

internal static class FromClauseParser
{
    private static string StartToken = "from";

    public static TableClause Parse(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();
        var startToken = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();

        if (startToken.ToLower() != StartToken) throw new InvalidProgramException($"First token must be '{StartToken}'.");

        var fromTable = ParseSingleTableClause(parser);

        while (parser.CurrentToken.ToRelationType() != RelationTypes.Undefined)
        {
            var joinTable = ParseSingleTableClause(parser);
            fromTable.SubTableClauses.Add(joinTable);
        }

        return fromTable;
    }

    private static TableClause ParseSingleTableClause(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();

        var token = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();
        var tp = token.ToRelationType();

        var setRelation = (TableClause t) =>
        {
            if (tp == RelationTypes.From || tp == RelationTypes.Cross) return t;
            if (parser.CurrentToken != "on") return t;

            t.RelationClause = new RelationGroupParser(parser).Parse();
            return t;
        };

        token = q.First();
        if (token == "(")
        {
            token = q.First(); //inner text
            using var p = new SqlParser(token);
            var t = new TableClause() { RelationType = tp };
            t.RelationClause.IsDecorateBracket = false;

            t.SubSelectClause = p.ParseSelectQuery();

            q.First(); //skip inner sql token

            if (parser.CurrentToken != ")") throw new SyntaxException("From clause syntax errpr.");

            q.First(); //skip ')' token

            t.AliasName = parser.ParseAlias();

            setRelation(t);
            return t;
        }
        else
        {
            var t = new TableClause() { RelationType = tp };
            t.RelationClause.IsDecorateBracket = false;

            t.TableName = token;
            q.First();

            if (!parser.TableBreakTokens.Contains(parser.CurrentToken) && parser.CurrentToken.ToLower() != "on")
            {
                t.AliasName = parser.ParseAlias();
            }
            if (t.AliasName.IsEmpty()) t.AliasName = t.TableName;

            setRelation(t);
            return t;
        }
    }
}