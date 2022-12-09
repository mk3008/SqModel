using SqModel.Core.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class ExistsExpression : QueryContainer
{
    public ExistsExpression(IQueryCommandable query) : base(query)
    {
    }

    public override string GetCurrentCommandText() => "exists (" + Query.GetCommandText() + ")";
}
