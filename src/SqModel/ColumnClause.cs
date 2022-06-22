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
public class ColumnClause : IParameterCollection
{
    /// <summary>
    /// Table name.
    /// Specify an alias for the table clause.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Column name.
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// SQL command text.
    /// Specify a command to get a value other than a simple column value, such as an operation result.
    /// </summary>
    public string CommandText { get; set; } = string.Empty;

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
        if (ColumnName == "*") return String.Empty;
        return (AliasName != String.Empty) ? AliasName : ColumnName;
    }

    private string GetCommandText()
    {
        if (CommandText != string.Empty) return CommandText;
        if (TableName != String.Empty) return $"{TableName}.{ColumnName}";
        return ColumnName;
    }


    public Query ToQuery()
    {
        var name = GetName();
        var alias = (name != string.Empty && name != ColumnName) ? $" as {name}" : string.Empty;

        return new Query() { CommandText = $"{GetCommandText()}{alias}", Parameters = Parameters };
    }
}
