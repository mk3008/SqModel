using SqModel.Analysis.Extensions;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public class CaseExpressionParser : TokenReader
{
    public CaseExpressionParser(string text) : base(text)
    {
    }

    public static CaseExpression Parse(string text)
    {
        using var p = new CaseExpressionParser(text);
        return p.ParseExpression();
    }

    private static List<WhenExpression> ParseWhenExpressions(string text)
    {
        using var p = new CaseExpressionParser(text);
        return p.ParseWhenExpressions().ToList();
    }

    public CaseExpression ParseExpression()
    {
        if (PeekToken().AreEqual("case")) ReadToken();

        var cndtext = ReadUntilToken("when");

        CaseExpression? c = null;
        if (string.IsNullOrEmpty(cndtext))
        {
            c = new CaseExpression();
        }
        else
        {
            var cnd = SelectQueryParser.ParseValue(cndtext);
            c = new CaseExpression(cnd);
        }

        var exptext = ReadUntilToken("end");
        foreach (var w in ParseWhenExpressions("when " + exptext + " end"))
        {
            c.WhenExpressions.Add(w);
        }
        return c;
    }

    private IEnumerable<WhenExpression> ParseWhenExpressions()
    {
        var token = PeekToken();

        while (token.AreEqual("when"))
        {
            var (x, y) = ParseWhenExpression();
            token = y;
            yield return x;
        }

        if (token.AreEqual("else"))
        {
            var val = SelectQueryParser.ParseValue(ReadUntilToken("end"));
            yield return new WhenExpression(val);
        }
    }

    private (WhenExpression exp, string breaktoken) ParseWhenExpression()
    {
        var breaktoken = string.Empty;
        var fn = (string t) =>
        {
            if (t.AreEqual("when") || t.AreEqual("else") || t.AreEqual("end"))
            {
                breaktoken = t;
                return true;
            }
            return false;
        };

        var cnd = SelectQueryParser.ParseValue(ReadUntilToken("then"));
        var val = SelectQueryParser.ParseValue(ReadUntilToken(fn));
        var exp = new WhenExpression(cnd, val);
        return (exp, breaktoken);
    }
}