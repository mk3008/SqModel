using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Expression;

public class ValueExpression : IValueClause
{
    public List<IValueClause> Collection { get; } = new();

    public string Conjunction { get; set; } = String.Empty;

    public void AddParameter(string name, object? value)
        => throw new NotSupportedException();

    public Query ToQuery()
    {
        var q = new Query();
        Collection.ForEach(x => q = q.Merge(x.ToQuery()));
        q.InsertToken(Conjunction);
        if (q == null) throw new NullReferenceException();
        return q;
    }

    public string GetName() => string.Empty;
}