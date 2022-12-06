using SqModel.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Tables;

public class VirtualTable : TableBase
{
    public VirtualTable(IQueryCommand query)
    {
        Query = query;
    }

    public IQueryCommand Query { get; init; }

    public override string GetCommandText()
    {
        return "(\r\n" + Query.GetCommandText().InsertIndent() + "\r\n)";
    }
}