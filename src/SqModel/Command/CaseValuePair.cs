using SqModel.Building;
using SqModel.Clause;
using SqModel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Command;

public class CaseValuePair : IThenCommand
{
    public ICommand? WhenValue { get; set; } = null;

    public ICommand? ThenValue { get; set; } = null;

    private static string PrefixToken { get; set; } = "when";

    private static string SufixToken { get; set; } = "then";

    private static string OmitToken { get; set; } = "else";

    public Query ToQuery()
    {
        if (ThenValue == null) throw new InvalidProgramException();

        Query? q = null;

        // when value then
        if (WhenValue != null) q = WhenValue.ToQuery().Decorate(PrefixToken, SufixToken);
        // else 
        else q = new Query() { CommandText = OmitToken };

        // ... value
        q = q.Merge(ThenValue.ToQuery());
        return q;
    }
}

public static class CaseValuePairExtension
{
    public static CaseValuePair When(this CaseValuePair source, TableClause table, string column)
    => source.When(CommandBuilder.Create(table, column));

    public static CaseValuePair When(this CaseValuePair source, string table, string column)
    => source.When(CommandBuilder.Create(table, column));

    public static CaseValuePair When(this CaseValuePair source, object commandtext)
    => source.When(CommandBuilder.Create(commandtext));

    public static CaseValuePair When(this CaseValuePair source, ICommand value)
    {
        source.WhenValue = value;
        return source;
    }
}


