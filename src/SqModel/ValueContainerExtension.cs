using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class ValueContainerExtension
{
    public static ValueClause Equal(this ValueContainer source, TableClause table, string column)
        => source.Expression("=", ValueBuilder.ToValue(table, column));

    public static ValueClause Equal(this ValueContainer source, string table, string column)
        => source.Expression("=", ValueBuilder.ToValue(table, column));

    public static ValueClause Equal(this ValueContainer source, string value)
        => source.Expression("=", ValueBuilder.ToValue(value));

    public static ValueClause NotEqual(this ValueContainer source, TableClause table, string column)
        => source.Expression("<>", ValueBuilder.ToValue(table, column));

    public static ValueClause NotEqual(this ValueContainer source, string table, string column)
        => source.Expression("<>", ValueBuilder.ToValue(table, column));

    public static ValueClause NotEqual(this ValueContainer source, string value)
        => source.Expression("<>", ValueBuilder.ToValue(value));

    public static ValueClause IsNull(this ValueContainer source)
        => source.Expression("is", ValueBuilder.GetNullValue());

    public static ValueClause IsNotNull(this ValueContainer source)
        => source.Expression("is", ValueBuilder.GetNotNullValue());

    public static ValueClause Expression(this ValueContainer source, string sign, ValueClause value)
    {
        var c = new ValueConjunction() { Sign = sign };
        source.ValueConjunction = c;
        c.Destination = value;
        return value;
    }
}
