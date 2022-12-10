using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Tables;

public class VirtualTable : TableBase
{
    public VirtualTable(IQueryCommandable query)
    {
        Query = query;
    }

    public IQueryCommandable Query { get; init; }

    public override string GetCommandText()
    {
        return "(\r\n" + Query.GetCommandText().InsertIndent() + "\r\n)";
    }

    public override IDictionary<string, object?> GetParameters()
    {
        return Query.GetParameters();
    }
}