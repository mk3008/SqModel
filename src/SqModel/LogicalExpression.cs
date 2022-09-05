using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class LogicalExpression : ILogicalExpression
{
    public IValueClause? Left { get; set; } = null;

    public IValueClause? Right { get; set; } = null;

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
        => source.SetLeftValue(ValueBuilder.Create(commandtext));

    public static LogicalExpression Column(this LogicalExpression source, TableClause table, string column)
        => source.SetLeftValue(ValueBuilder.Create(table, column));

    public static LogicalExpression Column(this LogicalExpression source, string table, string column)
        => source.SetLeftValue(ValueBuilder.Create(table, column));

    public static LogicalExpression SetLeftValue(this LogicalExpression source, IValueClause value)
    {
        source.Left = value;
        return source;
    }

    public static void Equal(this LogicalExpression source, TableClause table, string column)
        => source.SetRightCommand("=", ValueBuilder.Create(table, column));

    public static void Equal(this LogicalExpression source, string table, string column)
        => source.SetRightCommand("=", ValueBuilder.Create(table, column));

    public static IValueClause Equal(this LogicalExpression source, object commandtext)
        => source.SetRightCommand("=", ValueBuilder.Create(commandtext));

    public static void NotEqual(this LogicalExpression source, TableClause table, string column)
        => source.SetRightCommand("<>", ValueBuilder.Create(table, column));

    public static void NotEqual(this LogicalExpression source, string table, string column)
        => source.SetRightCommand("<>", ValueBuilder.Create(table, column));

    public static IValueClause NotEqual(this LogicalExpression source, object commandtext)
        => source.SetRightCommand("<>", ValueBuilder.Create(commandtext));

    public static void IsNull(this LogicalExpression source)
        => source.SetRightCommand("is", ValueBuilder.GetNullValue());

    public static void IsNotNull(this LogicalExpression source)
        => source.SetRightCommand("is", ValueBuilder.GetNotNullValue());

    public static void True(this LogicalExpression source)
        => source.SetRightCommand("=", ValueBuilder.GetTrue());

    public static void False(this LogicalExpression source)
        => source.SetRightCommand("=", ValueBuilder.GetFalse());

    private static IValueClause SetRightCommand(this LogicalExpression source, string conjunction, IValueClause value)
    {
        source.Right = value;
        source.Right.Conjunction = conjunction;
        return source.Right;
    }
}