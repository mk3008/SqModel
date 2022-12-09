namespace SqModel.Core.Extensions;

public static class NullSortTypeExtension
{
    public static string ToCommandText(this NullSortType source)
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