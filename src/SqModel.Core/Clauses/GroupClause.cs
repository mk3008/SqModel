using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public class GroupClause : IQueryCommand
{
    public GroupClause(ValueCollection condition)
    {
        Values = condition;
    }

    public ValueCollection Values { get; init; }

    public string GetCommandText()
    {
        /*
         * group by
         *     col1,
         *     col2
         */
        return $"group by\r\n" + Values.GetCommandText().InsertIndent();
    }
}