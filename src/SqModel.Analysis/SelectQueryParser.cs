using SqModel.Analysis.Extensions;
using SqModel.Core;
using SqModel.Core.Clauses;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public class SelectQueryParser : TokenReader
{
    public SelectQueryParser(string text) : base(text)
    {
    }

    public void Parse()
    {
        var token = ReadToken();
        //if (!(token.AreEqual("select") || token.AreEqual("with"))) throw new SyntaxException($"near {token} (expect: select or with)");

        if (token.AreEqual("select"))
        {

        }
    }

    public SelectableItem ParseSelectItem()
    {
        var aliastoken = "as";
        var breaktokens = new string[] { ",", "from", "where", "group by", "having", "order by", "union" };
        var operatorTokens = new string[] { "+", "-", "*", "/", "=", "!=", "<>", ">=", "<=", "||" };

        var item = ReadToken();
        IValue? value = null;
        var name = string.Empty;

        //table.column
        if (PeekToken().AreEqual("."))
        {
            var table = item;
            ReadToken();
            name = ReadToken();
            value = new ColumnValue(table, name);
        }

        if (value == null) throw new Exception();

        if (PeekToken().AreContains(breaktokens))
        {
            return new SelectableItem(value, name);
        }

        if (PeekToken().AreContains(operatorTokens))
        {

        }

        if (PeekToken().AreEqual(aliastoken))
        {
            ReadToken();
            name = ReadToken();
        }

        return new SelectableItem(value, name);
    }

    public SelectableItem ParseSelectableItem()
    {
        var breaktokens = new string?[] { null, ",", "from", "where", "group by", "having", "order by", "union" };

        var v = ParseValue();

        if (PeekToken().AreContains(breaktokens))
        {
            return new SelectableItem(v, v.GetDefaultName());
        }

        if (PeekToken().AreEqual("as"))
        {
            ReadToken();
            if (PeekToken().AreContains(breaktokens))
            {
                throw new SyntaxException($"alias name is not found.");
            }
        }

        return new SelectableItem(v, ReadToken());
    }

    public ValueBase ParseValue()
    {
        var breaktokens = new string?[] { null, "as", ",", "from", "where", "group by", "having", "order by", "union" };
        var operatorTokens = new string[] { "+", "-", "*", "/", "=", "!=", "<>", ">=", "<=", "||", "&", "|", "^", "#", "~" };

        var item = ReadToken();
        ValueBase? value = null;

        if (item.IsNumeric() || item.StartsWith("'") || item.AreEqual("true") || item.AreEqual("false"))
        {
            value = new LiteralValue(item);
        }
        else if (item == "(")
        {
            var (inner, closer) = ReadUntilCloseBracket();
            if (inner.IsSelectQuery())
            {
                //TODO : inline query
                throw new NotSupportedException();
            }
            else
            {
                using var p = new SelectQueryParser(inner);
                value = new BracketValue(p.ParseValue());
            }
        }
        else if (item == "case")
        {
            //TODO : case expression
            throw new NotSupportedException();
        }
        else if (PeekToken().AreEqual("("))
        {
            ReadToken();
            var (inner, closer) = ReadUntilCloseBracket();
            value = new FunctionValue(item, inner);
        }
        else if (PeekToken().AreEqual("."))
        {
            //table.column
            var table = item;
            ReadToken();
            value = new ColumnValue(table, ReadToken());
        }
        else
        {
            //omit table column
            value = new ColumnValue(item);
        }

        if (value == null) throw new Exception();

        if (PeekToken().AreContains(breaktokens))
        {
            return value;
        }

        if (PeekToken().AreEqual("over"))
        {
            var ov = ReadToken();
            if (!PeekToken().AreEqual("(")) throw new Exception();
            ReadToken(); // "("
            var (inner, closer) = ReadUntilCloseBracket();
            var v = new BracketValue(new LiteralValue(inner));
            value.AddOperatableValue(ov, v);
        }

        if (PeekToken().AreContains(operatorTokens))
        {
            var op = ReadToken();
            value.AddOperatableValue(op, ParseValue());
        }

        return value;
    }
}
