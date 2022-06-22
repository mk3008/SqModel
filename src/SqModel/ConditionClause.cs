using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ConditionClause: IParameterCollection
{
    public string CommandText { get; set; } = string.Empty;

    /// <summary>
    /// SQL Parameter.
    /// Specify the parameter name and value used in the command.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();

    public Query ToQuery()
    {
        return new Query() { CommandText = CommandText, Parameters = Parameters };
    }
}
