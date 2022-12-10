using Cysharp.Text;
using SqModel.Core.Extensions;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Clauses;

public class LimitClause : IQueryCommand, IQueryParameter
{
    public LimitClause(string text)
    {
        Conditions.Add(new LiteralValue(text));
    }

    public LimitClause(ValueBase item)
    {
        Conditions.Add(item);
    }

    public LimitClause(List<ValueBase> conditions)
    {
        conditions.ForEach(x => Conditions.Add(x));
    }

    public ValueCollection Conditions { get; init; } = new();

    public ValueBase? Offset { get; set; }

    public string GetCommandText()
    {
        /*
         * having
         *     sum(col1) = 1 and sum(col2) = 2
         */
        var sb = ZString.CreateStringBuilder();
        sb.Append("limit " + Conditions.GetCommandText());
        if (Offset != null) sb.Append(" offset " + Offset.GetCommandText());
        return sb.ToString();
    }

    public IDictionary<string, object?> GetParameters()
    {
        var prm = Conditions.GetParameters();
        prm = prm.Merge(Offset!.GetParameters());
        return prm;
    }
}