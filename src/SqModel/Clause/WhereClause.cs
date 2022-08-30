using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel;
using SqModel.CommandContainer;
using SqModel.Extension;

namespace SqModel.Clause;

public class WhereClause
{
    //public OperatorContainer Container { get; set; } = new() { Splitter = "\r\n", IsRoot = true };

    public ConditionGroup ConditionGroup { get; set; } = new() { IsOneLineFormat = false, IsDecorateBracket = false };

    public bool IsOneLineFormat { get; set; } = false;

    public Query ToQuery()
    {
        var q = new Query();
        q = ConditionGroup.ToQuery();
        if (q.IsEmpty()) return q;

        if (IsOneLineFormat)
        {
            q.CommandText = $"where {q.CommandText}";
        }
        else
        {
            q.CommandText = $"where\r\n{q.CommandText.InsertIndent()}";
        }
        return q;
    }
}

public static class WhereClauseExtension
{
    public static void SetOneLineFormat(this WhereClause source)
    {
        source.IsOneLineFormat = true;
        source.ConditionGroup.IsOneLineFormat = true;
    }
}