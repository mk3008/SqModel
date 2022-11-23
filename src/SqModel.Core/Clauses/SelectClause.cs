using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class SelectClause : IQueryable
{
    public List<SelectableItem> Items { get; set; } = new();

    public string GetCommandText()
    {
        if (!Items.Any()) throw new IndexOutOfRangeException(nameof(Items));

        /*
         * select
         *     col1 as c1,
         *     col2 as c2
         */
        return "select".Join($"\r\n", Items.Select(x => x.GetCommandText().InsertIndent()), $",\r\n");
    }

    public IDictionary<string, object?> GetParameters()
    {
        return Items.Select(x => x.GetParameters()).Merge();
    }
}