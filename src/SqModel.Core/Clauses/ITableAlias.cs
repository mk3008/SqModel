using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public interface ITableAlias
{
    string Alias { get; init; }

    List<string>? ColumnAliases { get; set; }
}

public static class ITableAliasExtension
{
    public static string GetAliasCommand(this ITableAlias source)
    {
        /*
         * alias(col1, col2, col3)
         */
        if (source.ColumnAliases == null || !source.ColumnAliases.Any()) return source.Alias;
        return source.Alias.Join("", source.ColumnAliases, ", ", "(", ")");
    }
}