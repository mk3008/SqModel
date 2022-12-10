using SqModel;
using SqModel.Extension;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public string ParseAlias()
    {
        Logger?.Invoke($"{nameof(ParseAlias)} start");

        var q = ReadTokensWithoutComment();

        var alias = CurrentToken.IsNotEmpty() ? CurrentToken : q.First();
        if (alias.IsEmpty()) return String.Empty;
        if (AliasTokens.Contains(alias)) alias = q.First();

        if (AliasBreakTokens.Where(x => x == CurrentToken).Any()) return String.Empty;

        q.First();
        return alias;
    }
}
