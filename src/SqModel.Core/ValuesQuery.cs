using Cysharp.Text;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core;

public class ValuesQuery : QueryBase, IQueryCommandable
{
    public ValuesClause? ValuesClause { get; set; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        if (ValuesClause == null) throw new InvalidProgramException();
        foreach (var item in ValuesClause.GetTokens()) yield return item;
    }
}