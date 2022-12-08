using SqModel.Analysis.Extensions;
using SqModel.Core.Clauses;
using SqModel.Core.Values;

namespace SqModel.Analysis.Parser;

public static class ValueParser
{
    public static ValueBase Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static ValueBase Parse(TokenReader r)
    {
        var operatorTokens = new string[] { "+", "-", "*", "/", "=", "!=", ">", "<", "<>", ">=", "<=", "||", "&", "|", "^", "#", "~", "and", "or" };

        ValueBase value = ParseCore(r);

        if (r.PeekToken().AreContains(operatorTokens))
        {
            var op = r.ReadToken();
            value.AddOperatableValue(op, Parse(r));
        }
        return value;
    }

    private static ValueBase ParseCore(TokenReader r)
    {
        var breaktokens = new string?[] { null, "as", ",", "from", "where", "group by", "having", "order by", "union" };

        var item = r.ReadToken();

        if (item.AreEqual("not"))
        {
            return new NegativeValue(Parse(r));
        }

        if (item.IsNumeric() || item.StartsWith("'") || item.AreEqual("true") || item.AreEqual("false"))
        {
            return new LiteralValue(item);
        }

        if (item == "(")
        {
            var (first, inner) = r.ReadUntilCloseBracket();
            if (first.AreEqual("select"))
            {
                return new InlineQuery(SelectQueryParser.Parse(inner));
            }
            return new BracketValue(Parse(inner));
        }

        if (item == "case")
        {
            var text = "case " + r.ReadUntilCaseExpressionEnd();
            return CaseExpressionParser.Parse(text);
        }

        if (item == "exists")
        {
            r.ReadToken("(");
            var (first, inner) = r.ReadUntilCloseBracket();
            if (first.AreEqual("select"))
            {
                return new ExistsExpression(SelectQueryParser.Parse(inner));
            }
            throw new SyntaxException($"near exists({first}");
        }

        if (r.PeekToken().AreEqual("("))
        {
            return FunctionValueParseLogic.Parse(r, item);
        }

        if (r.PeekToken().AreEqual("."))
        {
            //table.column
            var table = item;
            r.ReadToken(".");
            return new ColumnValue(table, r.ReadToken());
        }

        //omit table column
        return new ColumnValue(item);
    }
}