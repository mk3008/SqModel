using SqModel.Core.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class NegativeValue : ValueBase
{
    public NegativeValue(ValueBase inner)
    {
        Inner = inner;
    }

    public ValueBase Inner { get; init; }

    public override string GetCurrentCommandText()
    {
        if (Inner == null) throw new NullReferenceException();
        return "not " + Inner.GetCommandText();
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        return Inner.GetParameters();
    }
}
