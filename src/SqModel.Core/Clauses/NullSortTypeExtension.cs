namespace SqModel.Core.Clauses;

public static class NullSortTypeExtension
{
    public static string ToSortText(this NullSortType source)
    {
        switch (source)
        {
            case NullSortType.Undefined:
                return string.Empty;
            case NullSortType.First:
                return "nulls first";
            case NullSortType.Last:
                return "nulls last";
        }
        throw new NotSupportedException();
    }
}