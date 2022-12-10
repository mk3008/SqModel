using Cysharp.Text;
using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core;

public class ValuesQuery : QueryBase, IQueryCommandable
{
    public ValuesQuery(List<ValueCollection> rows)
    {
        Rows = rows;
    }

    public List<ValueCollection> Rows { get; init; } = new();

    public override string GetCurrentCommandText()
    {
        /*
         * values
         *     (v11, v12, v13),
         *     (v21, v22, v23),
         *     (v31, v32, v33)
         */
        if (!Rows.Any()) throw new IndexOutOfRangeException(nameof(Rows));
        var indent4 = 4.ToSpaceString();
        var isFirst = true;
        var sb = ZString.CreateStringBuilder();
        sb.Append("values\r\n");

        foreach (var item in Rows.Select(x => indent4 + "(" + x.GetCommandText() + ")"))
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                sb.Append(",\r\n");
            }
            sb.Append(item);
        }
        return sb.ToString();
    }

    public Dictionary<string, object?>? Parameters { get; set; }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(Parameters);
        return prm;
    }
}