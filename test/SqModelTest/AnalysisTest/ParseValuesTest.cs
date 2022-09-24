using Microsoft.VisualStudio.TestPlatform.Utilities;
using SqModel.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.AnalysisTest;

public class ParseValuesTest
{
    public ParseValuesTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        var text = "values (1,2,3), (4,5,6)";
        var c = ValuesClauseParser.Parse(text);
        var q = c.ToQuery();

        Assert.Equal(@"values
    (1, 2, 3)
    , (4, 5, 6)", q.CommandText);
    }
}