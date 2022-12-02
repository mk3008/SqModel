using SqModel.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class ValueCollection : ValueBase
{
    public List<ValueBase> Values { get; init; } = new();

    public override string GetCurrentCommandText()
    {
        return Values.Select(x => x.GetCommandText()).ToString(", ");
    }
}
