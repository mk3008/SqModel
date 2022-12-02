using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class FunctionValue : ValueBase
{
    public FunctionValue(string functionName, string argumentText)
    {
        FunctionName = functionName;
        Inner = new LiteralValue(argumentText);
    }

    public FunctionValue(string functionName, ValueBase argument)
    {
        FunctionName = functionName;
        Inner = argument;
    }

    public string FunctionName { get; init; }

    public override string GetCurrentCommandText()
    {
        if (Inner == null) throw new Exception();
        return $"{FunctionName}({Inner.GetCommandText()})";
    }
}
