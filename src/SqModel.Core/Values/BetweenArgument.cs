using SqModel.Core.Clauses;
using SqModel.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class BetweenArgument : ValueBase
{
    public BetweenArgument(ValueBase start, ValueBase end)
    {
        Start = start;
        End = end;
    }

    public ValueBase Start { get; init; }

    public ValueBase End { get; init; }

    public override string GetCurrentCommandText()
    {
        return Start.GetCommandText() + " and " + End.GetCommandText();
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        var prm = Start.GetCurrentParameters();
        prm = prm.Merge(End.GetCurrentParameters());
        return prm;
    }
}
