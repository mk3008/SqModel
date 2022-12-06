﻿using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class Relation : IQueryCommand
{
    public Relation(SelectableTable query, RelationType types)
    {
        Table = query;
        RelationType = types;
    }

    public Relation(SelectableTable query, RelationType types, IValue condition)
    {
        Table = query;
        RelationType = types;
        Condition = condition;
    }

    public RelationType RelationType { get; init; }

    public IValue? Condition { get; set; }

    public SelectableTable Table { get; init; }

    public string GetCommandText()
    {
        /*
         * inner join table as t2 on t1.id = t2.id
         */
        var cmd = $"{RelationType.ToRelationText()} {Table.GetCommandText()}";
        if (Condition == null) return cmd;
        return $"{cmd} on {Condition.GetCommandText()}";
    }
}