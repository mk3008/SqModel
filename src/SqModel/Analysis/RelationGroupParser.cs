using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Analysis;

internal class RelationGroupParser
{
    public RelationGroupParser(SqlParser parser)
    {
        Parser = parser;
    }

    private SqlParser Parser { get; init; }

    private static string StartToken = "on";

    public RelationGroup Parse()
    {
        var group = new RelationGroup() { IsDecorateBracket = false };

        var q = Parser.ReadTokensWithoutComment();
        var startToken = Parser.CurrentToken.IsNotEmpty() ? Parser.CurrentToken : q.First();

        if (startToken.ToLower() != StartToken) throw new InvalidProgramException($"First token must be '{StartToken}'.");
        q.First(); //skip start token token

        AddRelation(group, string.Empty);

        while (Parser.CurrentToken.IsLogicalOperator())
        {
            var @operator = Parser.CurrentToken;

            q.First();

            AddRelation(group, @operator);
        }
        return group;
    }

    public void AddRelation(RelationGroup group, string @operator)
    {
        var suboperator = string.Empty;

        var q = Parser.ReadTokensWithoutComment();

        if (Parser.CurrentToken.ToLower() == "not")
        {
            suboperator = Parser.CurrentToken;
            q.First();
        }

        if (Parser.CurrentToken == "(")
        {
            var subquery = Parser.ReadUntilCloseBracket();
            using var p = new SqlParser(subquery) { Logger = Parser.Logger };
            var xp = new RelationGroupParser(p);
            xp.AddRelation(group, @operator);
            return;
        }
        else if (Parser.CurrentToken.ToLower() == "exists")
        {
            var tmp = q.First();
            if (tmp != "(") throw new InvalidProgramException();

            var subquery = Parser.ReadUntilCloseBracket();
            using var p = new SqlParser(subquery) { Logger = Parser.Logger };
            var eq = p.ParseSelectQuery();
            eq.IsOneLineFormat = true;
            group.Add().SetOperator(@operator, suboperator).Exists(eq);
            return;
        }
        else
        {
            group.Add().SetOperator(@operator, suboperator).Expression = LogicalExpressionParser.Parse(Parser);
            return;
        }
    }
}