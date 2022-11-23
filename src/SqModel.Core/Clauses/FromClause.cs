using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class FromClause : IQueryable
{
    public FromClause(SelectableTable root)
    {
        Root = root;
    }

    public SelectableTable Root { get; init; }

    public List<Relation>? Relations { get; set; }

    public string GetCommandText()
    {
        /*
         * from
         *     table as t1
         *     inner join table as t2 on t1.id = t2.id
         *     inner join table as t3 on t1.id = t3.id
         */
        var indent = 4.ToSpaceString();
        var cmd = $"from\r\n{indent}{Root.GetCommandText()}";
        return cmd.Join($"\r\n", Relations!.Select(x => x.GetCommandText().InsertIndent()), $"\r\n");
    }

    public IDictionary<string, object?> GetParameters()
    {
        var prm = Root.GetParameters();
        if (Relations == null) return prm;
        var relationprm = Relations.Select(x => x.GetParameters()).Merge();
        return prm.Merge(relationprm);
    }
}