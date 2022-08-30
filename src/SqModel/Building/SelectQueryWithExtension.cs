﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.CommandContainer;

namespace SqModel.Building;

public static class SelectQueryWithExtension
{
    public static CommonTable With(this SelectQuery source, Action<SelectQuery> action, string alias)
    {
        var sq = new SelectQuery();
        action(sq);
        return source.With.Add(sq, alias);
    }

    public static CommonTable With(this SelectQuery source, Func<SelectQuery> fn, string alias)
        => source.With.Add(fn(), alias);

    public static CommonTable With(this SelectQuery source, SelectQuery commonQuery, string alias)
        => source.With.Add(commonQuery, alias);
}
