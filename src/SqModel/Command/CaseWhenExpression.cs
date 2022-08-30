using SqModel.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Command;

public class CaseWhenExpression : ICommand
{
    public List<CaseWhenValuePair> Collection { get; set; } = new();

    internal static string PrefixToken { get; set; } = "case";

    internal static string SufixToken { get; set; } = "end";

    public string Conjunction { get ; set; }= String.Empty;

    public Query ToQuery()
    {
        var q = new Query();
        Collection.ForEach(x => q = q.Merge(x.ToQuery()));
        q = q.Decorate(PrefixToken, SufixToken).InsertToken(Conjunction);
        return q;
    }

    public void AddParameter(string name, object value)
        => throw new NotSupportedException();
}

public static class CaseWhenExpressionExtension
{
    public static CaseWhenValuePair Add(this CaseWhenExpression source)
    {
        var c = new CaseWhenValuePair();
        source.Collection.Add(c);
        return c;
    }
}