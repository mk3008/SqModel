using SqModel;
using SqModel.Analysis;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest;

public class Element
{
    public Element(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        //using SqModel;
        var sq = SqlParser.Parse("select a.id, a.id as col, 1+1 as val from a");

        var a = sq.FromClause;
        sq.Select.Add().Column(a, "column1");
        sq.Select.Add().Column(a, "column2").As("column2");
        sq.Select.Add().Column(a, "column3").As("col3");

        sq.GetSelectItems().ForEach(x =>
        {
            Output.WriteLine($"> {x.ToQuery().CommandText}");
            Output.WriteLine($"  name : {x.Name}");
            Output.WriteLine($"  column name : {x.ColumnName}");
            Output.WriteLine($"  command text : {x.ToCommandText()}");

            var expect = string.Empty;
            if (x.Command != null)
            {
                expect = x.Command.ToQuery().CommandText;
            }
            Assert.Equal(expect, x.ToCommandText());
        });
    }
}
