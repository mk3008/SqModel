namespace SqModel.Core.Clauses;

public static class RelationTypeExtension
{
    public static string ToRelationText(this RelationType source)
    {
        switch (source)
        {
            case RelationType.From:
                return "from";
            case RelationType.Inner:
                return "inner join";
            case RelationType.Left:
                return "left join";
            case RelationType.Right:
                return "right join";
            case RelationType.Cross:
                return "cross join";
        }
        throw new NotSupportedException();
    }
}