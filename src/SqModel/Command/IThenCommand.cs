using SqModel.Building;
using SqModel.Clause;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Command;

public interface IThenCommand
{
    ICommand? ThenValue { get; set; }
}

public static class IThenCommandExtension
{
    public static void Then(this IThenCommand source, TableClause table, string column)
        => source.Then(CommandBuilder.Create(table, column));

    public static void Then(this IThenCommand source, string table, string column)
        => source.Then(CommandBuilder.Create(table, column));

    public static void Then(this IThenCommand source, object commandtext)
        => source.Then(CommandBuilder.Create(commandtext));

    public static void Then(this IThenCommand source, ICommand value)
        => source.ThenValue = value;
}
