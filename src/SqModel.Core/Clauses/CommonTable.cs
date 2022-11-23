using System.Text;
using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class CommonTable : IQueryable, ITableAlias
{
    public CommonTable(ITable query, string alias)
    {
        Query = query;
        Alias = alias;
    }

    public ITable Query { get; init; }

    public Dictionary<string, object?>? Parameters { get; set; } = null;

    public List<string>? Keywords { get; set; }

    public string Alias { get; init; }

    public List<string>? ColumnAliases { get; set; }

    public string GetCommandText()
    {
        /*
         * alias(columns) as keyword (
         *     query
         * )
         */
        var query = Query.GetCommandText();
        var alias = this.GetAliasCommand();
        var keyword = Keywords == null || !Keywords.Any() ? string.Empty : Keywords.ToString(" ");

        var sb = new StringBuilder();
        sb.Append($"{alias} as ");
        if (string.IsNullOrEmpty(keyword))
        {
            sb.Append($"{keyword} ");
        }
        sb.Append($"(\r\n{query.InsertIndent()}\r\n)");
        return sb.ToString();
    }

    public IDictionary<string, object?> GetParameters()
    {
        if (Parameters == null) return Query.GetParameters();
        return Parameters.Merge(Query.GetParameters());
    }
}