using Cysharp.Text;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class CaseExpression : ValueBase
{
    public CaseExpression()
    {
    }

    public CaseExpression(ValueBase condition)
    {
        CaseCondition = condition;
    }

    public ValueBase? CaseCondition { get; init; }

    public List<WhenExpression> WhenExpressions { get; init; } = new();

    public override string GetCurrentCommandText()
    {
        var sb = ZString.CreateStringBuilder();
        sb.Append("case");
        if (CaseCondition != null) sb.Append(" " + CaseCondition.GetCommandText());
        foreach (var item in WhenExpressions)
        {
            sb.Append(" " + item.GetCommandText());
        }
        sb.Append(" end");
        return sb.ToString();
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(CaseCondition!.GetParameters());
        WhenExpressions.ForEach(x => prm = prm.Merge(x.GetParameters()));
        return prm;
    }
}