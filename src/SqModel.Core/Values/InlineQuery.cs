﻿using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class InlineQuery : QueryContainer
{
    public InlineQuery(IQueryable query) : base(query)
    {
    }

    public override string GetCurrentCommandText() => "(" + Query.GetCommandText() + ")";
}