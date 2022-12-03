using SqModel.Analysis.Extensions;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis.Builder;

public static class ValueBuilder
{
    public static ValueBase Build(string text)
    {
        using var r = new TokenReader(text);
        return Build(r);
    }

    public static ValueBase Build(TokenReader r)
    {
        var breaktokens = new string?[] { null, "as", ",", "from", "where", "group by", "having", "order by", "union" };
        var operatorTokens = new string[] { "+", "-", "*", "/", "=", "!=", "<>", ">=", "<=", "||", "&", "|", "^", "#", "~" };

        var item = r.ReadToken();
        ValueBase? value = null;

        if (item.IsNumeric() || item.StartsWith("'") || item.AreEqual("true") || item.AreEqual("false"))
        {
            value = new LiteralValue(item);
        }
        else if (item == "(")
        {
            var (_, inner) = r.ReadUntilCloseBracket();
            if (inner.IsSelectQuery())
            {
                //TODO : inline query
                throw new NotSupportedException();
            }
            else
            {
                value = new BracketValue(Build(inner));
            }
        }
        else if (item == "case")
        {
            var text = "case " + r.ReadUntilCaseExpressionEnd();
            value = CaseExpressionParser.Parse(text);
        }
        else if (r.PeekToken().AreEqual("("))
        {
            value = FunctionValueBuilder.Build(r, item);
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

        //if (r.PeekToken().AreEqual("over"))
        //{
        //    var ov = r.ReadToken();
        //    if (!r.PeekToken().AreEqual("(")) throw new SyntaxException("near over. expect '('");
        //    r.ReadToken(); // "("
        //    var (_, inner) = r.ReadUntilCloseBracket();

        //    var winfn = r.ParseWindowFuncion(inner);
        //    value.AddOperatableValue("", winfn);
        //}

        if (r.PeekToken().AreContains(operatorTokens))
        {
            var op = r.ReadToken();
            value.AddOperatableValue(op, Build(r));
        }

        return value;
    }
}
