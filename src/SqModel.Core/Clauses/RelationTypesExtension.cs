namespace SqModel.Core.Clauses;

public static class RelationTypesExtension
{
    public static string ToRelationText(this RelationTypes source)
    {
        switch (source)
        {
            case RelationTypes.From:
                return "from";
            case RelationTypes.Inner:
                return "inner join";
            case RelationTypes.Left:
                return "left join";
            case RelationTypes.Right:
                return "right join";
            case RelationTypes.Cross:
                return "cross join";
        }
        throw new NotSupportedException();
    }
}