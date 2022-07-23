using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest.SqlSerializerTest;

public class ParseTest
{
    public ParseTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Full()
    {
        using var p = new SelectQueryParser(@"
select
    a.column_1 as col1
    , a.column_2 as col2
    , ((1+2) * 3) as col3
    , (select b.value from table_b b) as b_value
    , ' comment('')comment ' as comment /* prefix /* nest */ sufix */
from
    table_a as a
    inner join table_c as c on a.column_1 = c.column_1
    left outer join table_d as d on a.column_2 = d.column_2 and a.column_3 = d.column_3
    left join table_e as e on a.column_4 = e.column_4
where
    a.column_1 = 1
order by 
    a.column_1");
        p.Logger = (x) => Output.WriteLine(x);

        var lst = p.ReadAllTokens().ToList();
    }
}
