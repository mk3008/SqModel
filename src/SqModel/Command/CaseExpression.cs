using SqModel.Building;
using SqModel.CommandContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Command;

public class CaseExpression : ICommand
{
    public ICommand? Value { get; set; } = null;

    public List<CaseValuePair> Collection { get; set; } = new();

    private static string PrefixToken { get; set; } = "case";

    private static string SufixToken { get; set; } = "end";

    public string Conjunction { get; set; } = String.Empty;

    public Query ToQuery()
    {
        var q = Value?.ToQuery();
        q ??= new();
        Collection.ForEach(x => q = q.Merge(x.ToQuery()));
        q = q.Decorate(PrefixToken, SufixToken).InsertToken(Conjunction);
        return q;
    }

    public void AddParameter(string name, object value)
        => throw new NotSupportedException();
}

public static class CaseExpressionExtension
{
    public static CaseValuePair Add(this CaseExpression source)
    {
        var c = new CaseValuePair();
        source.Collection.Add(c);
        return c;
    }
}