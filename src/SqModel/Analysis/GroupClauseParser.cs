namespace SqModel.Analysis;

public static class GroupClauseParser
{
    private static string StartToken = "group by";

    public static NamelessItemClause Parse(string text)
    {
        using var p = new SqlParser(text);
        return Parse(p);
    }

    public static NamelessItemClause Parse(SqlParser parser)
        => NamelessItemsParser.Parse(parser, StartToken);
}