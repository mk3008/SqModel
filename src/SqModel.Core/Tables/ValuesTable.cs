﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Tables;

public class ValuesTable : TableBase
{
    public ValuesTable(List<ValueCollection> rows)
    {
        Rows = rows;
    }

    public List<ValueCollection> Rows { get; init; } = new();

    public override string GetCommandText()
    {
        /*
         * values
         *     (v11, v12, v13),
         *     (v21, v22, v23),
         *     (v31, v32, v33)
         */
        if (!Rows.Any()) throw new IndexOutOfRangeException(nameof(Rows));
        var indent4 = 4.ToSpaceString();
        var indent8 = 8.ToSpaceString();
        var isFirst = true;
        var sb = ZString.CreateStringBuilder();
        sb.Append("(\r\n" + indent4 + "values\r\n");

        foreach (var item in Rows.Select(x => indent8 + "(" + x.GetCommandText() + ")"))
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                sb.Append(",\r\n");
            }
            sb.Append(item);
        }
        sb.Append("\r\n)");

        return sb.ToString();
    }
}