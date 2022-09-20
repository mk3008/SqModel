using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel;
using SqModel.Extension;

namespace SqModel;

public class ConditionClause
{
    public ConditionClause(string token)
    {
        Token = token;
    }

    public string Token { get; init; }

    public ConditionGroup ConditionGroup { get; set; } = new() { IsOneLineFormat = false, IsDecorateBracket = false };

    public bool IsOneLineFormat { get; set; } = false;

    public Query ToQuery()
    {
        var q = new Query();
        q = ConditionGroup.ToQuery();
        if (q.IsEmpty()) return q;

        if (IsOneLineFormat)
        {
            q.CommandText = $"{Token} {q.CommandText}";
        }
        else
        {
            q.CommandText = $"{Token}\r\n{q.CommandText.InsertIndent()}";
        }
        return q;
    }
}