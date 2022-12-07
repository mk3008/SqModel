using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class WhereClause : IQueryCommand, IQueryParameter
{
    public WhereClause(ValueBase condition)
    {
        Condition = condition;
    }

    public ValueBase Condition { get; init; }

    public string GetCommandText()
    {
        /*
         * where
         *     col1 = 1 and col2 = 2
         */
        return $"where\r\n" + Condition.GetCommandText().InsertIndent();
    }

    public IDictionary<string, object?> GetParameters()
    {
        return Condition.GetParameters();
    }
}