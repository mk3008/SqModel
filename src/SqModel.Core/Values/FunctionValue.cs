﻿using Cysharp.Text;
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

    public override string GetCurrentCommandText()
    {
        var sb = ZString.CreateStringBuilder();
        sb.Append(Name + "(");
        if (Argument != null) sb.Append(Argument.GetCommandText());
        sb.Append(")");
        if (WindowFunction != null) sb.Append(" " + WindowFunction.GetCommandText());
        return sb.ToString();
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(Argument?.GetParameters());
        prm = prm.Merge(WindowFunction?.GetParameters());
        return prm;
    }
}