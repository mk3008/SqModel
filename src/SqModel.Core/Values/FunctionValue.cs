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
        ArgumentText = argumentText;
    }

    public string FunctionName { get; init; }

    public string ArgumentText { get; init; }

    internal override string GetCurrentCommandText() => $"{FunctionName}({ArgumentText})";
}
