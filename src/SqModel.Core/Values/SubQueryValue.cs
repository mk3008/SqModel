﻿using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class SubQueryValue : ValueBase
{
    public SubQueryValue(IQueryable query)
    {
        Query = query;
    }

    public IQueryable Query { get; init; }

    internal override string GetCurrentCommandText() => $"({Query.GetCommandText()})";
}