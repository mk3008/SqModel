using SqModel.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.CommandContainer;

public class ExistsExpression : ILogicalExpression
{
    public SelectQuery? Query { get; set; } = null;

    public Query ToQuery()
    {
        if (Query == null) throw new InvalidProgramException();
        Query.IsincludeCte = false;
        //Query.IsOneLineFormat = true;
        return Query.ToQuery().DecorateBracket().InsertToken("exists");
    }
}
