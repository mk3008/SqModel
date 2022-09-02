using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public WhereClause ParseWhereClause()
    {
        Logger?.Invoke($"{nameof(ParseWhereClause)} start");

        var w = new WhereClause();
        w.ConditionGroup = ParseConditionGroup(); ;

        Logger?.Invoke($"{nameof(ParseWhereClause)} end : {w.ToQuery().CommandText}");
        return w;
    }
}
