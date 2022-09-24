using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public static class ValuesClauseParser
{
    private static string StartToken = "values";

    public static ValuesClause Parse(string text)
    {
        using var p = new SqlParser(text);
        var c = Parse(p);
        return c;
    }

    public static ValuesClause Parse(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();

        var token = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();
        if (token.ToLower() != StartToken) throw new InvalidProgramException();
   
        var c = new ValuesClause();
        var fn = () =>
        {
            q.First();
            if (parser.CurrentToken != "(") throw new InvalidProgramException();
            q.First();
            c.Collection.Add(NamelessItemsParser.Parse(parser.CurrentToken));
            q.First();
            if (parser.CurrentToken != ")") throw new InvalidProgramException();
            q.First();
        };

        fn();
        while (parser.CurrentToken == ",")
        {
            fn();
        }
        return c;
    }
}