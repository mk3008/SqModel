using SqModel.Analysis.Extensions;
using SqModel.Core.Values;

namespace SqModel.Analysis.Parser;

public static class FunctionValueParseLogic
{
    public static FunctionValue Parse(TokenReader r, string functionName)
    {
        r.ReadToken("(");
        var (_, argstext) = r.ReadUntilCloseBracket();
        var arg = ValueCollectionParser.Parse(argstext);

        if (!r.PeekToken().AreEqual("over"))
        {
            return new FunctionValue(functionName, arg);
        }

        r.ReadToken("over");
        var winfn = WindowFunctionParser.Parse(r);
        return new FunctionValue(functionName, arg, winfn);
    }
}