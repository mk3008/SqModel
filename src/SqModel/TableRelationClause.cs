namespace SqModel;


public class TableRelationClause
{
    public RelationTypes RelationType { get; set; } = RelationTypes.Inner;

    public string SourceName { get; set; } = string.Empty;

    public JoinColumnRelationClause JoinColumnRelationClause { get; set; } = new();

    public TableRelationClause Add(string sourceColumn, string destinationColumn, string sign = "=")
    {
        JoinColumnRelationClause.ColumnRelationClauses.Add(new ColumnRelationClause()
        {
            SourceColumn = sourceColumn,
            DestinationColumn = destinationColumn,
            Sign = sign
        });
        return this;
    }

    public TableRelationClause Add(string column)
    {
        return Add(column, column, "=");
    }

    public Query ToQuery(string alias, Query tableQ)
    {
        var join = string.Empty;
        switch (RelationType)
        {
            case RelationTypes.Inner:
                join = "inner join";
                break;
            case RelationTypes.Left:
                join = "left  join";
                break;
            case RelationTypes.Right:
                join = "right join";
                break;
            case RelationTypes.Cross:
                join = "cross join";
                break;
        }

        var text = $"{join} {tableQ.CommandText}";
        var q = new Query() { CommandText = text, Parameters = tableQ.Parameters };

        if (RelationType == RelationTypes.Cross) return q;

        return q.Merge(JoinColumnRelationClause.ToQuery(SourceName, alias), " on ");
    }

    public enum RelationTypes
    {
        Inner = 0,
        Left = 1,
        Right = 2,
        Cross = 3,
    }
}