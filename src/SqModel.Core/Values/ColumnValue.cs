using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class ColumnValue : IValue
{
    public ColumnValue(string column)
    {
        Column = column;
    }

    public string? TableAlias { get; set; }

    public string Column { get; init; }

    public Dictionary<string, object?>? Parameters { get; set; }

    public NestedValue? Nest { get; set; }

    public string GetCommandText()
    {
        if (string.IsNullOrEmpty(TableAlias)) return Column;
        return $"{TableAlias}.{Column}";
    }

    public string? GetName() => Column;

    public IDictionary<string, object?> GetParameters()
    {
        if (Parameters != null) return Parameters;
        return EmptyParameters.Get();
    }
}