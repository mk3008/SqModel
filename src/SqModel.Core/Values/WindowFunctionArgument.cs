using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class WindowFunctionArgument : IQueryCommand
{
    public ValueCollection? PartitionBy { get; set; }

    public ValueCollection? OrderBy { get; set; }

    public string GetCommandText()
    {
        if (PartitionBy == null && OrderBy == null) throw new Exception();

        var sb = ZString.CreateStringBuilder();
        if (PartitionBy != null) sb.Append("partition by " + PartitionBy.GetCommandText());
        if (PartitionBy != null && OrderBy != null) sb.Append(" ");
        if (OrderBy != null) sb.Append("order by " + OrderBy.GetCommandText());

        return sb.ToString();
    }
}