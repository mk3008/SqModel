using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ColumnRelationClause 
{
    public string SourceColumn { get; set; } = String.Empty;

    public string DestinationColumn { get; set; } = String.Empty;

    public string Sign { get; set; } = "=";

    public Query ToQuery(string sourceTable, string destinationTable)
    {
        return new Query() { CommandText = $"{sourceTable}.{SourceColumn} {Sign} {destinationTable}.{DestinationColumn}", Parameters = new() };
    }
}