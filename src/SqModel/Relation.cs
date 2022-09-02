using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class Relation : Condition, IRelation
{
    public string LeftTable { get; set; } = string.Empty;

    public string RightTable { get; set; } = string.Empty;
}

public static class RelationConditionExtension
{
    public static void Equal(this Relation source, string column)
        => source.Column(source.LeftTable, column).Equal(source.RightTable, column);

    public static void Equal(this Relation source, string leftcolumn, string rightcolumn)
        => source.Column(source.LeftTable, leftcolumn).Equal(source.RightTable, rightcolumn);

    public static void Equal(this Relation source, Dictionary<string, string> columnmap)
    {
        foreach (var item in columnmap)
        {
            source.Column(source.LeftTable, item.Key).Equal(source.RightTable, item.Value);
        }
    }
}
