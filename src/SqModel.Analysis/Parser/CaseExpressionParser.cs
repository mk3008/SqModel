using SqModel.Analysis.Extensions;
using SqModel.Core.Values;

namespace SqModel.Analysis.Builder;

public static class CaseExpressionParser
{
    public static CaseExpression Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static CaseExpression Parse(TokenReader r)
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
            var cnd = ValueParser.Parse(cndtext);
            c = new CaseExpression(cnd);
        }

        var exptext = r.ReadUntilToken("end");
        foreach (var w in WhenExpressionParser.Parse("when " + exptext + " end"))
        {
            c.WhenExpressions.Add(w);
        }
        return c;
    }
}