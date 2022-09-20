using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public static class HavingClauseParser
{
    private static string StartToken = "having";

    public static ConditionClause Parse(string text)
    {
        using var p = new SqlParser(text);
        return Parse(p);
    }

    public static ConditionClause Parse(SqlParser parser)
    {
        var w = new ConditionClause(StartToken);
        w.ConditionGroup = ConditionGroupParser.Parse(parser, StartToken);
        w.ConditionGroup.IsDecorateBracket = false;
        w.ConditionGroup.IsOneLineFormat = false;
        return w;
    }
}