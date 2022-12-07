using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class BracketValue : ValueBase
{
    public BracketValue(ValueBase inner)
    {
        Inner = inner;
    }

    public ValueBase Inner { get; init; }

    public override string GetCurrentCommandText()
    {
        if (Inner == null) throw new NullReferenceException();
        return "(" + Inner.GetCommandText() + ")";
    }
}