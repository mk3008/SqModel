using SqModel.Core.Clauses;
using SqModel.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class BetweenExpression : ValueBase
{
    public BetweenExpression(ValueBase value, ValueBase start, ValueBase end)
    {
        Value = value;
        Start = start;
        End = end;
    }

    public ValueBase Value { get; init; }

    public ValueBase Start { get; init; }

    public ValueBase End { get; init; }

    public override string GetCurrentCommandText()
    {
        return Value.GetCommandText() + " between " + Start.GetCommandText() + " and " + End.GetCommandText();
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        var prm = Start.GetCurrentParameters();
        prm = prm.Merge(End.GetCurrentParameters());
        return prm;
    }
}