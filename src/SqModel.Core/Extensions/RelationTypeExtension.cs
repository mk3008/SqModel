namespace SqModel.Core.Extensions;

public static class RelationTypeExtension
{
    public static string ToCommandText(this RelationType source)
    {
        switch (source)
        {
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