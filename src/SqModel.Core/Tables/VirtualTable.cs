using SqModel.Core.Clauses;
using SqModel.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Tables;

public class VirtualTable : TableBase
{
    public VirtualTable(IQueryable query)
    {
        Query = query;
    }

    public IQueryable Query { get; init; }

    public override string GetCommandText()
    {
        return "(\r\n" + Query.GetCommandText().InsertIndent() + "\r\n)";
    }

    public override IDictionary<string, object?> GetParameters()
    {
        return Query.GetParameters();
    }
}