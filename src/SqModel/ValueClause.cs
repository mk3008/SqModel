using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

/// <summary>
/// Column clause.
/// Define the column to get.
/// </summary>
public class ValueClause : IParameterCollection
{
    /// <summary>
    /// Table name.
    /// Specify an alias for the table clause.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Value name.
    /// </summary>
    public string Value { get; set; } = string.Empty;
        
    public SelectQuery? InlineQuery { get; set; }

    /// <summary>
    /// Column alias name.
    /// </summary>
    public string AliasName { get; set; } = string.Empty;

    /// <summary>
    /// SQL Parameter.
    /// Specify the parameter name and value used in the command.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <summary>
    /// Get the column name.
    /// If all columns are specified, an empty string is returned.
    /// </summary>
    /// <returns></returns>
    public string GetName()
    {
        if (Value == "*") return String.Empty;
        return (AliasName != String.Empty) ? AliasName : Value;
    }

    public Query ToQuery()
    {
        var name = GetName();
        var alias = (name != string.Empty && name != Value) ? $" as {name}" : string.Empty;

        if (InlineQuery != null)
        {
            var q = InlineQuery.ToQuery();
            return new Query() { CommandText = $"({q.CommandText}){alias}", Parameters = q.Parameters };
        }

        if (TableName != String.Empty)
        {
            return new Query() { CommandText = $"{TableName}.{Value}{alias}", Parameters = Parameters };
        }

        return new Query() { CommandText = $"{Value}{alias}", Parameters = Parameters };
    }
}
