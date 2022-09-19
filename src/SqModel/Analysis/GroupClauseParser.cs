using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public static class GroupClauseParser
{
    private static string StartToken = "group by";

    public static NamelessItems Parse(string text)
    {
        using var p = new SqlParser(text);
        return Parse(p);
    }

    public static NamelessItems Parse(SqlParser parser)
        => NamelessItemsParser.Parse(parser, StartToken);
}