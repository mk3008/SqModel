namespace Carbunql.Core.Extensions;

public static class MaterializedTypeExtension
{
    public static string ToCommandText(this MaterializedType source)
    {
        switch (source)
        {
            case MaterializedType.Undefined:
                return string.Empty;
            case MaterializedType.Materialized:
                return "materialized";
            case MaterializedType.NotMaterialized:
                return "not materialized";
        }
        throw new NotSupportedException();
    }
}