using Carbunql.Core.Extensions;

namespace Carbunql.Core.Clauses;

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

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, RelationType.ToCommandText());
        foreach (var item in Table.GetTokens(parent)) yield return item;

        if (Condition != null)
        {
            yield return Token.Reserved(this, parent, "on");
            foreach (var item in Condition.GetTokens(parent)) yield return item;
        }
    }
}