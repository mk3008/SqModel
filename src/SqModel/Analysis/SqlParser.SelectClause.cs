﻿using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public SelectQuery ParseSelectQuery()
    {
        Logger?.Invoke($"{nameof(ParseSelectQuery)} start");

        var sq = new SelectQuery();

        var q = ReadTokensWithoutComment();
        if (CurrentToken.IsEmpty()) q.First();// read first token

        if (CurrentToken.ToLower() == "with")
        {
            sq.With = WithClauseParser.Parse(this);
        }
        if (CurrentToken.ToLower() == "select")
        {
            sq.SelectClause = SelectClauseParser.Parse(this);
        }
        if (CurrentToken.ToLower() == "from")
        {
            sq.FromClause = FromClauseParser.Parse(this);
        }
        if (CurrentToken.ToLower() == "where")
        {
            sq.WhereClause = WhereClauseParser.Parse(this);
        };

        return sq;
    }
}