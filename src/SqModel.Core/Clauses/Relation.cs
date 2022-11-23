using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class Relation : IQueryable
{
    public Relation(SelectableTable query, RelationTypes types)
    {
        Query = query;
        RelationType = types;
    }

    public RelationTypes RelationType { get; init; }

    public IQueryable? Condition { get; set; }

    public SelectableTable Query { get; init; }

    public string GetCommandText()
    {
        /*
         * inner join table as t2 on t1.id = t2.id
         */
        var cmd = $"{RelationType.ToRelationText()} {Query.GetCommandText()}";
        if (Condition == null) return cmd;
        return $"{cmd} on {Condition.GetCommandText()}";
    }

    public IDictionary<string, object?> GetParameters()
    {
        var prm = Query.GetParameters();
        if (Condition == null) return prm;
        return prm.Merge(Condition.GetParameters());
    }
}