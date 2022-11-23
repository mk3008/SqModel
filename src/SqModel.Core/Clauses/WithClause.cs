using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class WithClause : IQueryable
{
    public List<CommonTable> CommonTables { get; set; } = new();

    public string GetCommandText()
    {
        if (!CommonTables.Any()) throw new IndexOutOfRangeException(nameof(CommonTables));

        /*
         * with
         *     alias1 as (
         *         query
         *     ),
         *     alias2 as (
         *         query
         *     )
         */
        return "with".Join($"\r\n", CommonTables.Select(x => x.GetCommandText().InsertIndent()), $",\r\n");
    }

    public IDictionary<string, object?> GetParameters()
    {
        if (!CommonTables.Any()) throw new IndexOutOfRangeException(nameof(CommonTables));
        return CommonTables.Select(x => x.GetParameters()).Merge();
    }
}