using SqModel.Analysis.Extensions;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SqModel.Analysis.Builder;

public static class ValueCollectionBuilder
{
    public static ValueCollection Build(string text)
    {
        using var r = new TokenReader(text);
        return new ValueCollection(ReadValues(r).ToList());
    }

    public static ValueCollection Parse(TokenReader r)
    {
        return new ValueCollection(ReadValues(r).ToList());
    }

    private static IEnumerable<ValueBase> ReadValues(TokenReader r)
    {
        do
        {
            if (r.PeekToken().AreEqual(",")) r.ReadToken();
            yield return ValueBuilder.Build(r);
        }
        while (r.PeekToken().AreEqual(","));
    }
}
