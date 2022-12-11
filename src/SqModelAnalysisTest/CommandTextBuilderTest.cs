using SqModel.Analysis;
using SqModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SqModelAnalysisTest;

public class CommandTextBuilderTest
{
    private readonly ITestOutputHelper Output;

    public CommandTextBuilderTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private void LogOutput(List<string> arguments)
    {
        foreach (var item in arguments)
        {
            Output.WriteLine(item);
        }
    }

    [Fact]
    public void Default()
    {
        var lst = new List<(TokenType type, BlockType block, String text)>
        {
            (TokenType.Reserved, BlockType.BlockStart, "select"),

            (TokenType.Value, BlockType.Default, "a.column1"),
            (TokenType.Reserved, BlockType.Default, "as"),
            (TokenType.ValueName, BlockType.Default, "col1"),

            (TokenType.ValueSplitter, BlockType.Splitter, ","),

            (TokenType.Value, BlockType.Default, "a.column2"),
            (TokenType.Reserved, BlockType.Default, "as"),
            (TokenType.ValueName, BlockType.Default, "col2"),

            (TokenType.ValueSplitter, BlockType.Splitter, ","),

            (TokenType.Value, BlockType.Default, "a.column3"),
            (TokenType.Reserved, BlockType.Default, "as"),
            (TokenType.ValueName, BlockType.Default, "col3"),

            (TokenType.ValueSplitter, BlockType.Splitter, ","),

            (TokenType.Value, BlockType.Default, "1"),
            (TokenType.Operator, BlockType.Default, "+"),
            (TokenType.Value, BlockType.Default, "2"),
            (TokenType.Reserved, BlockType.Default, "as"),
            (TokenType.ValueName, BlockType.Default, "val1"),

            (TokenType.ValueSplitter, BlockType.Splitter, ","),
            (TokenType.Bracket, BlockType.BlockStart, "("),
            (TokenType.Value, BlockType.Default, "1"),
            (TokenType.Operator, BlockType.Default, "+"),
            (TokenType.Value, BlockType.Default, "2"),
            (TokenType.Bracket, BlockType.BlockEnd, ")"),
            (TokenType.Operator, BlockType.Default, "*"),
            (TokenType.Value, BlockType.Default, "3"),
            (TokenType.Reserved, BlockType.Default, "as"),
            (TokenType.ValueName, BlockType.Default, "val2"),

            (TokenType.Control, BlockType.BlockEnd, string.Empty),

            (TokenType.Reserved, BlockType.BlockStart, "from"),

            (TokenType.Table, BlockType.Default, "table_a"),
            (TokenType.Reserved, BlockType.Default, "as"),
            (TokenType.TableName, BlockType.Default, "a"),

            (TokenType.Control, BlockType.Splitter, string.Empty),

            (TokenType.Reserved, BlockType.Default, "inner join"),
            (TokenType.Table, BlockType.Default, "table_b"),
            (TokenType.Reserved, BlockType.Default, "as"),
            (TokenType.TableName, BlockType.Default, "b"),
            (TokenType.Reserved, BlockType.Default, "on"),
            (TokenType.Value, BlockType.Default, "a.table_a_id"),
            (TokenType.Operator, BlockType.Default, "="),
            (TokenType.Value, BlockType.Default, "b.table_a_id"),

            //(TokenType.TableStart, string.Empty),
            //(TokenType.Table, "table_b"),
            //(TokenType.Reserved, "as"),
            //(TokenType.TableName, "b"),
            //(TokenType.TableEnd, string.Empty),

            //(TokenType.RelationConditionStart, string.Empty),
            //(TokenType.Reserved, "on"),
            //(TokenType.ValueStart, string.Empty),
            //(TokenType.Value, "a.table_a_id"),
            //(TokenType.Operator, "="),
            //(TokenType.Value, "b.table_a_id"),
            //(TokenType.ValueEnd, string.Empty),
            //(TokenType.RelationConditionEnd, string.Empty),

            //(TokenType.RelationEnd, string.Empty),

            //(TokenType.RelationStart, string.Empty),

            //(TokenType.Reserved, "left join"),
            //(TokenType.TableStart, string.Empty),
            //(TokenType.Table, "table_c"),
            //(TokenType.Reserved, "as"),
            //(TokenType.TableName, "c"),
            //(TokenType.TableEnd, string.Empty),

            //(TokenType.RelationConditionStart, string.Empty),
            //(TokenType.Reserved, "on"),
            //(TokenType.ValueStart, string.Empty),
            //(TokenType.Value, "b.table_b_id"),
            //(TokenType.Operator, "="),
            //(TokenType.Value, "c.table_b_id"),
            //(TokenType.Operator, "and"),
            //(TokenType.Value, "b.table_b_sub_id"),
            //(TokenType.Operator, "="),
            //(TokenType.Value, "c.table_b_sub_id"),
            //(TokenType.ValueEnd, string.Empty),
            //(TokenType.RelationConditionEnd, string.Empty),

            //(TokenType.FromClauseEnd, string.Empty),
        };

        var sb = new CommandTextBuilder();
        Output.WriteLine(sb.Execute(lst));

        Output.WriteLine("----------");
        sb.DoSplitBefore = true;
        Output.WriteLine(sb.Execute(lst));

        Output.WriteLine("----------");
        sb.DoSplitBefore = true;
        sb.DoIndentInsideBracket = true;
        Output.WriteLine(sb.Execute(lst));

        Output.WriteLine("----------");
        sb.DoSplitBefore = true;
        sb.DoIndentInsideBracket = true;
        sb.AdjustFirstLineIndent = false;
        Output.WriteLine(sb.Execute(lst));

        //foreach (var item in lst)
        //{
        //    Output.WriteLine(Enum.GetName(item.type) + " : " + item.text);
        //}
        //foreach (var item in lst.Where(x => !string.IsNullOrEmpty(x.text)))
        //{
        //    Output.WriteLine(item.text);
        //}
    }
}
