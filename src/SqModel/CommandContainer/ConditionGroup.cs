using SqModel.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.CommandContainer;

public class ConditionGroup : IQueryable, ICondition
{
    public string Operator { get; set; } = "and";

    public string SubOperator { get; set; } = "";

    public List<ICondition> Collection { get; } = new();

    public bool IsDecorateBracket { get; set; } = true;

    public bool IsOneLineFormat { get; set; } = true;

    public Query ToQuery()
    {
        var splitter = (IsOneLineFormat) ? " " : "\r\n";
        var q = new Query();
        Collection.ForEach(x => q = q.Merge(x.ToQuery(), $"{splitter}{x.Operator} "));
        if (IsDecorateBracket && Collection.Count > 1) q = q.DecorateBracket();
        return q;
    }
}

public static class ConditionGroupExtension
{
    public static Condition Add(this ConditionGroup source)
    {
        var c = new Condition();
        source.Collection.Add(c);
        return c;
    }

    public static void AddGroup(this ConditionGroup source, Action<ConditionGroup> action)
    {
        var c = new ConditionGroup();
        source.Collection.Add(c);
        action(c);
    }
}