using SqModel.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.CommandContainer;

public class RelationGroup : IQueryable, IRelation
{
    public string Operator { get; set; } = "and";

    public string SubOperator { get; set; } = "";

    public List<IRelation> Collection { get; } = new();

    public string LeftTable { get; set; } = string.Empty;

    public string RightTable { get; set; } = string.Empty;

    public bool IsDecorateBracket { get; set; } = true;

    public string Splitter = " ";

    public Query ToQuery()
    {
        var q = new Query();
        Collection.ForEach(x => q = q.Merge(x.ToQuery(), $"{Splitter}{x.Operator} "));
        if (IsDecorateBracket && Collection.Count > 1) q = q.DecorateBracket();
        return q;
    }
}

public static class RelationGroupExtension
{
    public static Relation Add(this RelationGroup source)
    {
        var c = new Relation() { LeftTable = source.LeftTable, RightTable = source.RightTable };
        source.Collection.Add(c);
        return c;
    }

    public static void AddGroup(this RelationGroup source, Action<RelationGroup> action)
    {
        var c = new RelationGroup() { LeftTable = source.LeftTable, RightTable = source.RightTable };
        source.Collection.Add(c);
        action(c);
    }

    public static RelationGroup And(this RelationGroup source)
        => source.SetOperator("and");

    public static RelationGroup Or(this RelationGroup source)
        => source.SetOperator("or");

    internal static RelationGroup SetOperator(this RelationGroup source, string @operator)
    {
        source.Operator = @operator;
        return source;
    }

    internal static RelationGroup SetOperator(this RelationGroup source, string @operator, string suboperator)
    {
        source.Operator = @operator;
        source.SubOperator = suboperator;
        return source;
    }
}