using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class ValueListClause : IQueryable
{
    public ValueListClause(string command)
    {
        Command = command;
    }

    public string Command { get; init; }

    public List<IValue> Values { get; set; } = new();

    public string GetCommandText()
    {
        if (!Values.Any()) return string.Empty;

        /*
         * order by
         *     col1,
         *     col2
         */
        return Command.Join($"\r\n", Values.Select(x => x.GetCommandText().InsertIndent()), $",\r\n");
    }

    public IDictionary<string, object?> GetParameters()
    {
        if (!Values.Any()) return EmptyParameters.Get();
        return Values.Select(x => x.GetParameters()).Merge();
    }
}