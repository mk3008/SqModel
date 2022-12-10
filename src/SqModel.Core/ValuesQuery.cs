using Cysharp.Text;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core;

public class ValuesQuery : QueryBase, IQueryCommandable
{
    public ValuesClause? ValuesClause { get; set; }

    public override string GetCurrentCommandText()
    {
        if (ValuesClause == null) throw new InvalidProgramException();
        return ValuesClause.GetCommandText();
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(ValuesClause!.GetParameters());
        return prm;
    }
}