using SqModel.Analysis.Extensions;
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
        var breaktokens = new string?[] { null, "as", ",", "from", "where", "group by", "having", "order by", "union" };
        var operatorTokens = new string[] { "+", "-", "*", "/", "=", "!=", ">", "<", "<>", ">=", "<=", "||", "&", "|", "^", "#", "~", "and", "or" };

        var item = r.ReadToken();
        ValueBase? value = null;

        if (item.IsNumeric() || item.StartsWith("'") || item.AreEqual("true") || item.AreEqual("false"))
        {
            value = new LiteralValue(item);
        }
        else if (item == "(")
        {
            var (first, inner) = r.ReadUntilCloseBracket();
            if (first.AreEqual("select"))
            {
                //TODO : inline query
                throw new NotSupportedException();
            }
            else
            {
                value = new BracketValue(Parse(inner));
            }
        }
        else if (item == "case")
        {
            var text = "case " + r.ReadUntilCaseExpressionEnd();
            value = CaseExpressionParser.Parse(text);
        }
        else if (r.PeekToken().AreEqual("("))
        {
            value = FunctionValueParseLogic.Parse(r, item);
        }
        else if (r.PeekToken().AreEqual("."))
        {
            //table.column
            var table = item;
            r.ReadToken();
            value = new ColumnValue(table, r.ReadToken());
        }
        else
        {
            //omit table column
            value = new ColumnValue(item);
        }

        if (value == null) throw new Exception();

        if (r.PeekToken().AreContains(breaktokens))
        {
            return value;
        }

        if (r.PeekToken().AreContains(operatorTokens))
        {
            var op = r.ReadToken();
            value.AddOperatableValue(op, Parse(r));
        }

        return value;
    }
}