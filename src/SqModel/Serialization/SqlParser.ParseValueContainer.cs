﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public ValueContainer ParseValueContainer(bool includeCurrentToken = false)
    {
        Logger?.Invoke($"ValueContainer start");

        var c = new ValueContainer();

        c.Source = ParseValueClause(includeCurrentToken);
        c.SetSignValueClause(CurrentToken, ParseValueClause());

        Logger?.Invoke($"ValueContainer end : {c.ToQuery().CommandText}");
        return c;
    }
}