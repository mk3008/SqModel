using SqModel.Analysis.Builder;
using SqModel.Analysis.Extensions;
using SqModel.Core.Tables;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis.Parser;

public static class TableParser
{
    public static TableBase Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static TableBase Parse(TokenReader r)
    {
        var breaktokens = new string?[] { null, "as", "inner join", "left join", "left outer join", "right join", "right outer join", "cross join", "where", "group by", "having", "order by", "union" };

        var item = r.ReadToken();
        TableBase? value = null;

        if (item == "(")
        {
            var (first, inner) = r.ReadUntilCloseBracket();
            if (first.AreEqual("select"))
            {
                //TODO : subquery 
                throw new NotSupportedException();
            }
            else if (first.AreEqual("values"))
            {
                return ValuesTableParser.Parse(inner);
            }
            throw new NotSupportedException();
        }
        else if (r.PeekToken().AreEqual("."))
        {
            //schema.table
            var schema = item;
            r.ReadToken();
            value = new PhysicalTable(schema, r.ReadToken());
        }
        else
        {
            //table
            value = new PhysicalTable(item);
        }

        if (value == null) throw new Exception();

        return value;
    }
}