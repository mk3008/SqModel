using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Expression;

public class ConcatExpression : IValueClause
{
    public List<ValueContainer> Collection { get; } = new();

    public string Conjunction { get; set; } = String.Empty;

    public void AddParameter(string name, object value)
        => throw new NotSupportedException();

    public Query ToQuery()
    {
        var q = new Query();
        Collection.ForEach(x => q = q.Merge(x.ToQuery(), ", "));
        q.DecorateBracket().InsertToken("concat", "");
        return q;
    }

    public string GetName() => string.Empty;
}

public static class ConcatExpressionExtension
{
    public static ValueContainer Add(this ConcatExpression source)
    {
        var c = new ValueContainer();
        source.Collection.Add(c);
        return c;
    }
}
