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
        var item = r.ReadToken();

        if (item == "(")
        {
            //virtualTable
            var (first, inner) = r.ReadUntilCloseBracket();
            if (first.AreEqual("select"))
            {
                //TODO : subquery 
                throw new NotSupportedException();
            }
            else if (first.AreEqual("values"))
            {
                var vals = ValuesQueryParser.Parse(inner);
                return new VirtualTable(vals);
            }
        }
        else if (r.PeekToken().AreEqual("."))
        {
            //schema.table
            var schema = item;
            r.ReadToken();
            return new PhysicalTable(schema, r.ReadToken());
        }
        else
        {
            //table
            return new PhysicalTable(item);
        }

        throw new Exception();
    }
}