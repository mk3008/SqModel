using SqModel.Analysis.Extensions;
using SqModel.Core.Clauses;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis.Builder;

public class FunctionValueBuilder
{
    public static FunctionValue Build(TokenReader r, string name)
    {
        if (!r.PeekToken().AreEqual("(")) throw new SyntaxException($"near {name}. expect '('");

        r.ReadToken(); // read open brackert
        var (_, argstext) = r.ReadUntilCloseBracket();
        var arg = ValueCollectionBuilder.Build(argstext);

        if (!r.PeekToken().AreEqual("over"))
        {
            return new FunctionValue(name, arg);
        }

        r.ReadToken(); //read 'over' token
        var winfn = BuildeWindowFunction(r);
        return new FunctionValue(name, arg, winfn);

    }
    private static WindowFunction BuildeWindowFunction(TokenReader r)
    {
        if (!r.PeekToken().AreEqual("(")) throw new SyntaxException("near over. expect '('");
        r.ReadToken(); // read open brackert
        var (_, inner) = r.ReadUntilCloseBracket();
        return BuiildWindowFuncion(inner);
    }

    private static WindowFunction BuiildWindowFuncion(string text)
    {
        var dic = new Dictionary<string, ValueCollection>();

        using var r = new TokenReader(text);
        do
        {
            var token = r.ReadToken();
            var vs = ValueCollectionBuilder.Parse(r);
            dic.Add(token, vs);

        } while (r.PeekOrDefault() != null);

        var winfn = new WindowFunction();
        if (dic.ContainsKey("partition by")) winfn.PartitionBy = dic["partition by"];
        if (dic.ContainsKey("order by")) winfn.OrderBy = dic["order by"];

        return winfn;
    }
}
