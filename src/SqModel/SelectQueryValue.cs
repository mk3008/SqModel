using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class SelectQueryValue : IValueClause
{
    public SelectQuery? Query { get; set; } = null;

    public string Conjunction { get; set; } = string.Empty;

    public void AddParameter(string name, object value)
        => throw new NotSupportedException();

    public string GetName() => string.Empty;

    public Query ToQuery()
    {
        if (Query == null) throw new InvalidProgramException();
        Query.IsincludeCte = false;
        return Query.ToQuery().DecorateBracket().InsertToken(Conjunction);
    }
}