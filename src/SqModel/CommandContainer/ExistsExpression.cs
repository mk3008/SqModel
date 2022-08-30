using SqModel.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.CommandContainer;

public class ExistsExpression : SelectQuery, ILogicalExpression
{
    public override Query ToQuery()
    {
        return base.ToInlineQuery().DecorateBracket().InsertToken("exists");
    }
}
