using SqModel.Building;
using SqModel.Clause;
using SqModel.Command;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.CommandContainer;

public class LogicalExpression : ILogicalExpression
{
    public ICommand? Left { get; set; } = null;

    public ICommand? Right { get; set; } = null;

    public virtual Query ToQuery()
    {
        if (Left == null || Right == null) throw new InvalidProgramException();
        var q = Left.ToQuery();
        return q.Merge(Right.ToQuery());
    }
}

public static class LogicalExpressionExtension
{
    public static LogicalExpression Value(this LogicalExpression source, object commandtext)
=> source.SetLeftValue(CommandBuilder.Create(commandtext));

    public static LogicalExpression Column(this LogicalExpression source, TableClause table, string column)
        => source.SetLeftValue(CommandBuilder.Create(table, column));

    public static LogicalExpression Column(this LogicalExpression source, string table, string column)
        => source.SetLeftValue(CommandBuilder.Create(table, column));

    public static LogicalExpression SetLeftValue(this LogicalExpression source, ICommand value)
    {
        source.Left = value;
        return source;
    }
    
    public static void Equal(this LogicalExpression source, TableClause table, string column)
        => source.SetRightCommand("=", CommandBuilder.Create(table, column));

    public static void Equal(this LogicalExpression source, string table, string column)
        => source.SetRightCommand("=", CommandBuilder.Create(table, column));

    public static ICommand Equal(this LogicalExpression source, object commandtext)
        => source.SetRightCommand("=", CommandBuilder.Create(commandtext));

    public static void NotEqual(this LogicalExpression source, TableClause table, string column)
        => source.SetRightCommand("<>", CommandBuilder.Create(table, column));

    public static void NotEqual(this LogicalExpression source, string table, string column)
        => source.SetRightCommand("<>", CommandBuilder.Create(table, column));

    public static ICommand NotEqual(this LogicalExpression source, object commandtext)
        => source.SetRightCommand("<>", CommandBuilder.Create(commandtext));

    public static void IsNull(this LogicalExpression source)
        => source.SetRightCommand("is", CommandBuilder.GetNullValue());

    public static void IsNotNull(this LogicalExpression source)
        => source.SetRightCommand("is", CommandBuilder.GetNotNullValue());

    private static ICommand SetRightCommand(this LogicalExpression source, string conjunction, ICommand value)
    {
        source.Right = value;
        source.Right.Conjunction = conjunction;
        return source.Right;
    }
}