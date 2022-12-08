using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class InExpression : QueryContainer
{
    public InExpression(IQueryable query) : base(query)
    {
    }

    public override string GetCurrentCommandText() => "in (" + Query.GetCommandText() + ")";
}