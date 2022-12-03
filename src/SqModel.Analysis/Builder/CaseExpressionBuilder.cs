using SqModel.Analysis.Extensions;
using SqModel.Core.Values;

namespace SqModel.Analysis.Builder;

public class CaseExpressionBuilder
{
    public static CaseExpression Build(string text)
    {
        using var r = new TokenReader(text);
        return ParseExpression(r);
    }

    private static List<WhenExpression> BuildWhenExpressions(string text)
    {
        using var r = new TokenReader(text);
        return ParseWhenExpressions(r).ToList();
    }

    public static CaseExpression ParseExpression(TokenReader r)
    {
        if (r.PeekToken().AreEqual("case")) r.ReadToken();

        var cndtext = r.ReadUntilToken("when");

        CaseExpression? c = null;
        if (string.IsNullOrEmpty(cndtext))
        {
            c = new CaseExpression();
        }
        else
        {
            var cnd = ValueBuilder.Build(cndtext);
            c = new CaseExpression(cnd);
        }

        var exptext = r.ReadUntilToken("end");
        foreach (var w in BuildWhenExpressions("when " + exptext + " end"))
        {
            c.WhenExpressions.Add(w);
        }
        return c;
    }

    private static IEnumerable<WhenExpression> ParseWhenExpressions(TokenReader r)
    {
        var token = r.PeekToken();

        while (token.AreEqual("when"))
        {
            var (x, y) = ParseWhenExpression(r);
            token = y;
            yield return x;
        }

        if (token.AreEqual("else"))
        {
            var val = ValueBuilder.Build(r.ReadUntilToken("end"));
            yield return new WhenExpression(val);
        }
    }

    private static (WhenExpression exp, string breaktoken) ParseWhenExpression(TokenReader r)
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

        var cnd = ValueBuilder.Build(r.ReadUntilToken("then"));
        var val = ValueBuilder.Build(r.ReadUntilToken(fn));
        var exp = new WhenExpression(cnd, val);
        return (exp, breaktoken);
    }
}