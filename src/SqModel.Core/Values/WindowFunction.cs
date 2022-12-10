using Cysharp.Text;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class WindowFunction : IQueryCommand, IQueryParameter
{
    public ValueCollection? PartitionBy { get; set; }

    public ValueCollection? OrderBy { get; set; }

    public string GetCommandText()
    {
        if (PartitionBy == null && OrderBy == null) throw new Exception();

        var sb = ZString.CreateStringBuilder();
        sb.Append("over(");
        if (PartitionBy != null) sb.Append("partition by " + PartitionBy.GetCommandText());
        if (PartitionBy != null && OrderBy != null) sb.Append(" ");
        if (OrderBy != null) sb.Append("order by " + OrderBy.GetCommandText());
        sb.Append(")");

        return sb.ToString();
    }

    public IDictionary<string, object?> GetParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(PartitionBy!.GetParameters());
        prm = prm.Merge(OrderBy!.GetParameters());
        return prm;
    }
}