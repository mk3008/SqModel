using SqModel.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class Values : ValueBase
{
    public Values(List<ValueBase> collection)
    {
        Collection.AddRange(collection);
    }

    public List<ValueBase> Collection { get; init; } = new();

    public override string GetCurrentCommandText()
    {
        return Collection.Select(x => x.GetCommandText()).ToString(", ");
    }
}
