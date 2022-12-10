using SqModel.Extension;

namespace SqModel.Analysis;

public static class NamelessItemsParser
{
    public static NamelessItemClause Parse(string text, string starttoken = "")
    {
        using var p = new SqlParser(text);
        return Parse(p, starttoken);
    }

    public static NamelessItemClause Parse(SqlParser parser, string starttoken)
    {
        var q = parser.ReadTokensWithoutComment();

        if (starttoken.IsNotEmpty())
        {
            var startToken = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();
            if (startToken.ToLower() != starttoken) throw new SyntaxException($"First token must be '{starttoken}'.");
            q.First(); //skip start token token
        }

        var oc = new NamelessItemClause(starttoken);

        oc.Collection.Add(ParseOrderItem(parser));

        while (parser.CurrentToken == ",")
        {
            q.First();//skip ',' token
            oc.Collection.Add(ParseOrderItem(parser));
        }
        return oc;
    }

    public static NamelessItem ParseOrderItem(SqlParser parser)
    {
        var c = new NamelessItem();
        c.Command = ValueClauseParser.Parse(parser);
        return c;
    }
}
