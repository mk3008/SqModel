using SqModel.Analysis;
using SqModel.Analysis.Builder;
using SqModel.Core;
using SqModel.Core.Clauses;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class ValueParseTest
{
    private readonly ITestOutputHelper Output;

    public ValueParseTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private void LogOutput(IQueryCommand arguments)
    {
        Output.WriteLine($"{arguments.GetCommandText()}");
        Output.WriteLine("--------------------");
        LogOutputCore(arguments);
    }

    private void LogOutputCore(CaseExpression arguments, int indent = 0)
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
            LogOutputCore(arguments.CaseCondition, indent + 4);
        }

        if (arguments.WhenExpressions != null)
        {
            var s = (indent + 2).ToSpaceString();
            foreach (var item in arguments.WhenExpressions)
            {
                if (item.Condition != null)
                {
                    Output.WriteLine($"{s}when");
                    LogOutputCore(item, indent + 4);
                }
                else
                {
                    Output.WriteLine($"{s}else");
                    LogOutputCore(item, indent + 4);
                }
            }
        }

        if (arguments.OperatableValue != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.OperatableValue;
            Output.WriteLine($"{s}Operator : {v.Operator}");
            LogOutputCore(v.Value, indent + 4);
        }
    }

    private void LogOutputCore(WhenExpression arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

        if (arguments.Condition != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.Condition;
            Output.WriteLine($"{s}Condition");
            LogOutputCore(arguments.Condition, indent + 4);
        }

        if (arguments.Value != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.Condition;
            Output.WriteLine($"{s}Value");
            LogOutputCore(arguments.Value, indent + 4);
        }
    }


    private void LogOutputCore(ValueBase arguments, int indent = 0)
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
            LogOutputCore(v.Value, indent + 4);
        }
    }

    private void LogOutputCore(WindowFunction arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

        if (arguments.PartitionBy != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}partition by");
            LogOutputCore(arguments.PartitionBy, indent + 4);
        }

        if (arguments.OrderBy != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}order by");
            LogOutputCore(arguments.OrderBy, indent + 4);
        }
    }

    private void LogOutputCore(BracketValue arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

        if (arguments.Inner != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.OperatableValue;
            Output.WriteLine($"{s}Inner");
            LogOutputCore(arguments.Inner, indent + 4);
        }

        if (arguments.OperatableValue != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.OperatableValue;
            Output.WriteLine($"{s}Operator : {v.Operator}");
            LogOutputCore(v.Value, indent + 4);
        }
    }

    private void LogOutputCore(ValueCollection arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
        Output.WriteLine($"{space}Count : {arguments.Count}");

        foreach (var item in arguments)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}item");
            LogOutputCore(item, indent + 4);
        }
    }

    private void LogOutputCore(FunctionValue arguments, int indent = 0)
    {
        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");

        if (arguments.Argument != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}Arguments");
            LogOutputCore(arguments.Argument, indent + 4);
        }

        if (arguments.WindowFunction != null)
        {
            var s = (indent + 2).ToSpaceString();
            Output.WriteLine($"{s}WindowsFunction");
            LogOutputCore(arguments.WindowFunction, indent + 4);
        }

        if (arguments.OperatableValue != null)
        {
            var s = (indent + 2).ToSpaceString();
            var v = arguments.OperatableValue;
            Output.WriteLine($"{s}Operator : {v.Operator}");
            LogOutputCore(v.Value, indent + 4);
        }
    }

    private void LogOutputCore(IQueryCommand arguments, int indent = 0)
    {
        if (arguments is CaseExpression caseexp)
        {
            LogOutputCore(caseexp, indent);
            return;
        }
        else if (arguments is WindowFunction winfnarg)
        {
            LogOutputCore(winfnarg, indent);
            return;
        }
        else if (arguments is ValueCollection values)
        {
            LogOutputCore(values, indent);
            return;
        }
        else if (arguments is BracketValue bracket)
        {
            LogOutputCore(bracket, indent);
            return;
        }
        else if (arguments is FunctionValue fnvalue)
        {
            LogOutputCore(fnvalue, indent);
            return;
        }
        else if (arguments is ValueBase val)
        {
            LogOutputCore(val, indent);
            return;
        }

        var space = indent.ToSpaceString();

        Output.WriteLine($"{space}Type : {arguments.GetType().Name}");
        Output.WriteLine($"{space}Command : {arguments.GetCommandText()}");
    }

    [Fact]
    public void Column()
    {
        var text = "col";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("col", v.GetCommandText());
        Assert.Equal("col", v.GetDefaultName());
    }

    [Fact]
    public void TableColumn()
    {
        var text = "tbl.col";
        var v = ValueParser.Build(text);

        LogOutput(v);

        Assert.Equal("tbl.col", v.GetCommandText());
        Assert.Equal("col", v.GetDefaultName());
    }

    [Fact]
    public void Numeric()
    {
        var text = "3.14";
        var v = ValueParser.Build(text);

        LogOutput(v);

        Assert.Equal("3.14", v.GetCommandText());
    }

    [Fact]
    public void Text()
    {
        var text = "'abc''s'";
        var v = ValueParser.Build(text);

        LogOutput(v);

        Assert.Equal("'abc''s'", v.GetCommandText());
    }

    [Fact]
    public void BooleanTrue()
    {
        var text = "true";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("true", v.GetCommandText());
    }

    [Fact]
    public void BooleanFalse()
    {
        var text = "false";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("false", v.GetCommandText());
    }

    [Fact]
    public void Expression()
    {
        var text = "1*3.14";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("1 * 3.14", v.GetCommandText());
    }

    [Fact]
    public void Expression2()
    {
        var text = "tbl.col1 *   tbl.col2";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("tbl.col1 * tbl.col2", v.GetCommandText());
        Assert.Equal("", v.GetDefaultName());
    }

    [Fact]
    public void Expression3()
    {
        var text = "(1+1)*2";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("(1 + 1) * 2", v.GetCommandText());
    }

    [Fact]
    public void Function()
    {
        var text = "sum(tbl.col+    tbl.col2)";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("sum(tbl.col + tbl.col2)", v.GetCommandText());
    }

    [Fact]
    public void WindowFunction()
    {
        var text = "row_number() over(partition by tbl.col, tbl.col2 order by tbl.col3, tbl.col4)";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("row_number() over(partition by tbl.col, tbl.col2 order by tbl.col3, tbl.col4)", v.GetCommandText());
    }

    [Fact]
    public void WindowFunction2()
    {
        var text = "row_number() over(order by tbl.col, tbl.col2)";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("row_number() over(order by tbl.col, tbl.col2)", v.GetCommandText());
    }

    [Fact]
    public void CaseExpression()
    {
        var text = "case tbl.col when 1 then 10 when 2 then 20 else 30 end";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("case tbl.col when 1 then 10 when 2 then 20 else 30 end", v.GetCommandText());
    }

    [Fact]
    public void CaseWhenExpression()
    {
        var text = "case when tbl.col1 = 1 then 10 when tbl.col2 = 2 then 20 else 30 end";
        var v = ValueParser.Build(text);
        LogOutput(v);

        Assert.Equal("case when tbl.col1 = 1 then 10 when tbl.col2 = 2 then 20 else 30 end", v.GetCommandText());
    }
}
