using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public class HavingClause : IQueryCommand
{
    public HavingClause(IValue condition)
    {
        Condition = condition;
    }

    public IValue Condition { get; init; }

    public string GetCommandText()
    {
        /*
         * having
         *     sum(col1) = 1 and sum(col2) = 2
         */
        return $"having\r\n" + Condition.GetCommandText().InsertIndent();
    }
}