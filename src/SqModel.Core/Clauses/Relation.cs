using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class Relation : IQueryCommand
{
    public Relation(SelectableTable query, RelationType types)
    {
        Table = query;
        RelationType = types;
    }

    public Relation(SelectableTable query, RelationType types, ValueBase condition)
    {
        Table = query;
        RelationType = types;
        Condition = condition;
    }

    public RelationType RelationType { get; init; }

    public ValueBase? Condition { get; set; }

    public SelectableTable Table { get; init; }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        yield return (tp, RelationType.ToCommandText(), BlockType.Default, true);
        foreach (var item in Table.GetTokens()) yield return item;

        if (Condition != null)
        {
            yield return (tp, "on", BlockType.Default, true);
            foreach (var item in Condition.GetTokens()) yield return item;
        }
    }
}