using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ConditionGroup : IQueryable, ICondition
{
    public string Operator { get; set; } = "and";

    public string SubOperator { get; set; } = "";

    public List<ICondition> Collection { get; } = new();

    public string LeftTable { get; set; } = string.Empty;

    public string RightTable { get; set; } = string.Empty;

    public bool IsDecorateBracket { get; set; } = true;

    public bool IsOneLineFormat { get; set; } = true;

    public Query ToQuery()
    {
        var q = new Query();
        Collection.ForEach(x =>
        {
            var splitter = " ";
            if (IsOneLineFormat == false && x.Operator.ToLower() == "and") splitter = "\r\n";
            q = q.Merge(x.ToQuery(), $"{splitter}{x.Operator} ");
        });
        if (IsDecorateBracket) q = q.DecorateBracket();
        return q;
    }
}

public static class ConditionGroupExtension
{
    public static Condition Add(this ConditionGroup source)
    {
        var c = new Condition() { LeftTable = source.LeftTable, RightTable = source.RightTable };
        source.Collection.Add(c);
        return c;
    }

    public static void AddGroup(this ConditionGroup source, Action<ConditionGroup> action)
    {
        var c = new ConditionGroup() { LeftTable = source.LeftTable, RightTable = source.RightTable };
        source.Collection.Add(c);
        action(c);
    }

    public static ConditionGroup And(this ConditionGroup source)
    => source.SetOperator("and");

    public static ConditionGroup Or(this ConditionGroup source)
        => source.SetOperator("or");

    public static ConditionGroup Not(this ConditionGroup source)
        => source.SetSubOperator("not");

    private static ConditionGroup SetSubOperator(this ConditionGroup source, string suboperator)
    {
        source.SubOperator = suboperator;
        return source;
    }

    internal static ConditionGroup SetOperator(this ConditionGroup source, string @operator)
    {
        source.Operator = @operator;
        return source;
    }

    internal static ConditionGroup SetOperator(this ConditionGroup source, string @operator, string suboperator)
    {
        source.Operator = @operator;
        source.SubOperator = suboperator;
        return source;
    }
}