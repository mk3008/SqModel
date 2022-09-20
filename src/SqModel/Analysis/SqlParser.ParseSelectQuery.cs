using SqModel.Extension;
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
            sq.WithClause = WithClauseParser.Parse(this);
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
        if (CurrentToken.ToLower() == "group by")
        {
            sq.GroupClause = GroupClauseParser.Parse(this);
        };
        if (CurrentToken.ToLower() == "having")
        {
            sq.HavingClause = HavingClauseParser.Parse(this);
        };
        if (CurrentToken.ToLower() == "union")
        {
            sq.UnionClause = UnionClauseParser.Parse(this);
        };
        if (CurrentToken.ToLower() == "order by")
        {
            sq.OrderClause = OrderClauseParser.Parse(this);
        };

        return sq;
    }
}
