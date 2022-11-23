using System;
using System.Collections.Generic;
using System.Linq;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Tables;

internal class LiteralTable : ITable
{
    public List<LiteralRow> Rows { get; set; } = new();

    public string GetCommandText()
    {
        /*
         * values
         *     (v11, v12, v13),
         *     (v21, v22, v23),
         *     (v31, v32, v33)
         */
        if (!Rows.Any()) throw new IndexOutOfRangeException(nameof(Rows));
        var indent = 4.ToSpaceString();
        return "values".Join($"\r\n", Rows.Select(x => x.GetCommandText().InsertIndent()), $",\r\n");
    }

    public IDictionary<string, object?> GetParameters()
    {
        return Rows.Select(x => x.GetParameters()).Merge();
    }
}