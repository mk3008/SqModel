using Microsoft.VisualStudio.TestPlatform.Utilities;
using SqModel.Core.Values;
using SqModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using SqModel.Core.Clauses;
using SqModel.Core.Tables;

namespace SqModelAnalysisTest;
internal class QueryCommandMonitor
{
    private readonly ITestOutputHelper Output;

    public QueryCommandMonitor(ITestOutputHelper output)
    {
        Output = output;
    }

    public void Log(IQueryCommand arguments)
    {
        Output.WriteLine($"{arguments.GetCommandText()}");
        Output.WriteLine("--------------------");
        LogCore(arguments);
    }


    private void LogCore(IQueryCommand arguments, int indent = 0)
    {
        if (arguments is CaseExpression caseexp)
        {
            LogCore(caseexp, indent);
            return;
        }
        else if (arguments is WindowFunction winfnarg)
        {
            LogCore(winfnarg, indent);
            return;
        }
        else if (arguments is ValueCollection values)
        {
            LogCore(values, indent);
            return;
        }
        else if (arguments is BracketValue bracket)
        {
            LogCore(bracket, indent);
            return;
        }
        else if (arguments is FunctionValue fnvalue)
        {
            LogCore(fnvalue, indent);
            return;
        }
        else if (arguments is ValueBase val)
        {
            LogCore(val, indent);
            return;
        }
        else if (arguments is SelectableItem selectable)
        {
            LogCore(selectable, indent);
            return;
        }
        else if (arguments is SelectClause selectclauese)
        {
            LogCore(selectclauese, indent);
            return;
        }
        else if (arguments is PhysicalTable physicalTable)
        {
            LogCore(physicalTable, indent);
            return;
        }
        else if (arguments is ValuesTable valuesTable)
        {
            LogCore(valuesTable, indent);
            return;
        }
        else if (arguments is SelectableTable selectableTable)
        {
            LogCore(selectableTable, indent);
            return;
        }
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
    }

    private void LogCore(SelectableTable arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

        if (arguments.ColumnAliases != null)
        {
            foreach (var item in arguments.ColumnAliases)
            {
                var s = (indent + 2).ToSpaceString();
                Output.WriteLine($"{s}column name");
                LogCore(item, indent + 4);
            }
        }

        if (arguments.Table != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}Table");
            LogCore(arguments.Table, indent + 4);
        }
    }

    private void LogCore(ValuesTable arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
        Output.WriteLine($"{space}DefaultName : {arguments.GetDefaultName()}");

        foreach (var item in arguments.Rows)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}row");
            LogCore(item, indent + 4);
        }
    }

    private void LogCore(PhysicalTable arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
        Output.WriteLine($"{space}DefaultName : {arguments.GetDefaultName()}");
    }

    private void LogCore(SelectClause arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
        Output.WriteLine($"{space}Count : {arguments.Count}");

        foreach (var item in arguments)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}item");
            LogCore(item, indent + 4);
        }
    }

    private void LogCore(SelectableItem arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
        Output.WriteLine($"{space}Alias : {arguments.Alias}");

        if (arguments.Value != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}Value");
            LogCore(arguments.Value, indent + 4);
        }
    }

    private void LogCore(CaseExpression arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
        Output.WriteLine($"{space}CurrentCommand : {arguments.GetCurrentCommandText()}");
        Output.WriteLine($"{space}DefaultName : {arguments.GetDefaultName()}");

        if (arguments.CaseCondition != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}Condition");
            LogCore(arguments.CaseCondition, indent + 4);
        }

        if (arguments.WhenExpressions != null)
        {
            var s = (indent + 2).ToSpaceString();
            foreach (var item in arguments.WhenExpressions)
            {
                if (item.Condition != null)
                {
                    Output.WriteLine($"{s}when");
                    LogCore(item, indent + 4);
                }
                else
                {
                    Output.WriteLine($"{s}else");
                    LogCore(item, indent + 4);
                }
            }
        }

        if (arguments.OperatableValue != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.OperatableValue;
            Output.WriteLine($"{s}Operator : {v.Operator}");
            LogCore(v.Value, indent + 4);
        }
    }

    private void LogCore(WhenExpression arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

        if (arguments.Condition != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.Condition;
            Output.WriteLine($"{s}Condition");
            LogCore(arguments.Condition, indent + 4);
        }

        if (arguments.Value != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.Condition;
            Output.WriteLine($"{s}Value");
            LogCore(arguments.Value, indent + 4);
        }
    }


    private void LogCore(ValueBase arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
        Output.WriteLine($"{space}CurrentCommand : {arguments.GetCurrentCommandText()}");
        Output.WriteLine($"{space}DefaultName : {arguments.GetDefaultName()}");

        if (arguments.OperatableValue != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.OperatableValue;
            Output.WriteLine($"{s}Operator : {v.Operator}");
            LogCore(v.Value, indent + 4);
        }
    }

    private void LogCore(WindowFunction arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

        if (arguments.PartitionBy != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}partition by");
            LogCore(arguments.PartitionBy, indent + 4);
        }

        if (arguments.OrderBy != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}order by");
            LogCore(arguments.OrderBy, indent + 4);
        }
    }

    private void LogCore(BracketValue arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

        if (arguments.Inner != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.OperatableValue;
            Output.WriteLine($"{s}Inner");
            LogCore(arguments.Inner, indent + 4);
        }

        if (arguments.OperatableValue != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.OperatableValue;
            Output.WriteLine($"{s}Operator : {v.Operator}");
            LogCore(v.Value, indent + 4);
        }
    }

    private void LogCore(ValueCollection arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
        Output.WriteLine($"{space}Count : {arguments.Count}");

        foreach (var item in arguments)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}item");
            LogCore(item, indent + 4);
        }
    }

    private void LogCore(FunctionValue arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

        if (arguments.Argument != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}Arguments");
            LogCore(arguments.Argument, indent + 4);
        }

        if (arguments.WindowFunction != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}WindowsFunction");
            LogCore(arguments.WindowFunction, indent + 4);
        }

        if (arguments.OperatableValue != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.OperatableValue;
            Output.WriteLine($"{s}Operator : {v.Operator}");
            LogCore(v.Value, indent + 4);
        }
    }
}