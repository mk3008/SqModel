using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class FunctionValue : ValueBase
{
    public FunctionValue(string name, string argument)
    {
        Name = name;
        Inner = new LiteralValue(argument);
    }

    public FunctionValue(string functionName, IQueryCommand argument)
    {
        Name = functionName;
        Inner = argument;
    }

    public string Name { get; init; }

    public override string GetCurrentCommandText()
    {
        if (Inner == null) throw new Exception();
        return $"{Name}({Inner.GetCommandText()})";
    }
}
