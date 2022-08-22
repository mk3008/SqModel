using SqModel.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class RelationContainer : OperatorContainer
{
    public override RelationContainer And() => (RelationContainer)base.And();

    public override RelationContainer Or() => (RelationContainer)base.Or();

    public override RelationContainer Not() => (RelationContainer)base.Not();

    public override RelationContainer Add()
    {
        var c = new RelationContainer() { SourceName = SourceName, DestinationName = DestinationName };
        Add(c);
        return c;
    }

    public void Group(Action<RelationContainer> fn)
    {
        var group = new RelationContainer() { Operator = "and", SourceName = SourceName, DestinationName = DestinationName };
        Add(group);
        fn(group);
    }

    public void Equal(string column)
    {
        Condition = new() { Source = ValueBuilder.ToValue(SourceName, column) };
        Condition.SetSignValueClause("=", ValueBuilder.ToValue(DestinationName, column));
    }

    public void Equal(string sourcecolumn, string destinationcolumn)
    {
        Condition = new() { Source = ValueBuilder.ToValue(SourceName, sourcecolumn) };
        Condition.SetSignValueClause("=", ValueBuilder.ToValue(DestinationName, destinationcolumn));
    }

    public void Equal(Dictionary<string, string> columnmap)
    {
        foreach (var item in columnmap)
        {
            Condition = new() { Source = ValueBuilder.ToValue(SourceName, item.Key) };
            Condition.SetSignValueClause("=", ValueBuilder.ToValue(DestinationName, item.Value));
        }
    }
}
