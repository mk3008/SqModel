using SqModel.Analysis.Extensions;
using SqModel.Core.Clauses;

namespace SqModel.Analysis.Parser;

public static class SelectableItemParser
{
    public static SelectableItem Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SelectableItem Parse(TokenReader r)
    {
        var breaktokens = new string?[] { null, ",", "from", "where", "group by", "having", "order by", "union" };

        var v = ValueParser.Parse(r);

        if (r.PeekRawToken().AreContains(breaktokens))
        {
            return new SelectableItem(v, v.GetDefaultName());
        }

        r.TryReadToken("as");


        if (r.PeekRawToken().AreContains(breaktokens))
        {
            throw new SyntaxException($"alias name is not found.");
        }

        return new SelectableItem(v, r.ReadToken());
    }
}