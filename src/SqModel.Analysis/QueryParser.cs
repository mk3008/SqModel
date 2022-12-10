using SqModel.Analysis.Extensions;
using SqModel.Analysis.Parser;
using SqModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public static class QueryParser
{
    public static QueryBase Parse(string text)
    {
        using var r = new TokenReader(text);
        if (r.TryReadToken("with") != null) WithClauseParser.Parse(r);
        if (r.PeekRawToken().AreEqual("select")) return SelectQueryParser.Parse(r);
        if (r.PeekRawToken().AreEqual("values")) return ValuesQueryParser.Parse(r);

        throw new NotSupportedException();
    }
}
