using Cysharp.Text;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core;

public class SelectQuery : QueryBase, IQueryCommandable
{
    public SelectClause? SelectClause { get; set; }

    public FromClause? FromClause { get; set; }

    public WhereClause? WhereClause { get; set; }

    public GroupClause? GroupClause { get; set; }

    public HavingClause? HavingClause { get; set; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        if (SelectClause == null) yield break;

        foreach (var item in SelectClause.GetTokens()) yield return item;
        if (FromClause != null) foreach (var item in FromClause.GetTokens()) yield return item;
        if (WhereClause != null) foreach (var item in WhereClause.GetTokens()) yield return item;
        if (GroupClause != null) foreach (var item in GroupClause.GetTokens()) yield return item;
        if (HavingClause != null) foreach (var item in HavingClause.GetTokens()) yield return item;
    }
}