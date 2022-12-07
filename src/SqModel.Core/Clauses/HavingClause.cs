using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class HavingClause : IQueryCommand
{
    public HavingClause(ValueBase condition)
    {
        Condition = condition;
    }

    public ValueBase Condition { get; init; }

    public string GetCommandText()
    {
        /*
         * having
         *     sum(col1) = 1 and sum(col2) = 2
         */
        return $"having\r\n" + Condition.GetCommandText().InsertIndent();
    }
}