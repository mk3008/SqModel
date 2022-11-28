namespace SqModel.Core.Clauses;

public class WhereClause
{
    public WhereClause(IValue condition)
    {
        Condition = condition;
    }

    public IValue Condition { get; init; }

    public string GetCommandText()
    {
        /*
         * where
         *     col1 = 1 and col2 = 2
         */
        return $"where\r\n{Condition.GetCommandText()}";
    }

    //public IDictionary<string, object?> GetParameters() => Condition.GetParameters();
}