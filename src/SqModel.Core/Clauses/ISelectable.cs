using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public interface ISelectable
{
    string Alias { get; init; }
}

//public static class ITableAliasExtension
//{
//    public static string GetAliasCommand(this ISelectable source)
//    {
//        /*
//         * alias(col1, col2, col3)
//         */
//        if (source.ColumnAliases == null || !source.ColumnAliases.Any()) return source.Alias;
//        return source.Alias.Join("", source.ColumnAliases, ", ", "(", ")");
//    }
//}