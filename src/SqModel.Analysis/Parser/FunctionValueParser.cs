using SqModel.Analysis.Extensions;
using SqModel.Core.Values;

namespace SqModel.Analysis.Builder;

public static class FunctionValueParseLogic
{
    public static FunctionValue Parse(TokenReader r, string functionName)
    {
        if (!r.PeekToken().AreEqual("(")) throw new SyntaxException($"near {functionName}. expect '('");

        r.ReadToken(); // read open brackert
        var (_, argstext) = r.ReadUntilCloseBracket();
        var arg = ValueCollectionParser.Parse(argstext);

        if (!r.PeekToken().AreEqual("over"))
        {
            return new FunctionValue(functionName, arg);
        }

        r.ReadToken(); //read 'over' token
        var winfn = WindowFunctionParser.Parse(r);
        return new FunctionValue(functionName, arg, winfn);
    }
}