using Cysharp.Text;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class FunctionValue : ValueBase
{
    public FunctionValue(string name, string arg)
    {
        Name = name;
        Argument = new ValueCollection(arg);
    }

    public FunctionValue(string functionName, ValueCollection args)
    {
        Name = functionName;
        Argument = args;
    }

    public FunctionValue(string functionName, ValueCollection args, WindowFunction winfn)
    {
        Name = functionName;
        Argument = args;
        WindowFunction = winfn;
    }

    public string Name { get; init; }

    public ValueCollection? Argument { get; init; }

    public WindowFunction? WindowFunction { get; init; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        yield return (tp, Name, BlockType.Default, true);
        yield return (tp, "(", BlockType.Default, true);
        if (Argument != null)
        {
            foreach (var item in Argument.GetTokens()) yield return item;
        }
        yield return (tp, ")", BlockType.Default, true);
        if (WindowFunction != null)
        {
            foreach (var item in WindowFunction.GetTokens()) yield return item;
        }
    }
}