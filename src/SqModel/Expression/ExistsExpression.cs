using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Expression;

public class ExistsExpression : ILogicalExpression
{
    public SelectQuery? Query { get; set; } = null;

    public Query ToQuery()
    {
        if (Query == null) throw new InvalidProgramException();
        Query.IsIncludeCte = false;
        //Query.IsOneLineFormat = true;
        return Query.ToQuery().DecorateBracket().InsertToken("exists");
    }
}
