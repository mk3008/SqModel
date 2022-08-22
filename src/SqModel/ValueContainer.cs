using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ValueContainer : ISignValueClauseSettable<ValueClause>
{
    public ValueClause? Source { get; set; } = null;

    public ValueConjunction? ValueConjunction { get; set; } = null;

    public SelectQuery? ExistsQuery { get; set; } = null;

    public Query ToQuery()
    {
        if (ExistsQuery != null) return ExistsQuery.ToQuery().InsertIndent().DecorateBracket("\r\n").InsertToken("exists");

        if (Source != null && ValueConjunction != null)
        {
            var sq = Source.ToQuery();
            var ds = ValueConjunction.ToQuery();
            sq = sq.Merge(ds, $" {ValueConjunction.Sign} ");
            return sq;
        }

        return new Query();
    }

    public ValueClause SetSignValueClause(string sign, ValueClause value)
    {
        ValueConjunction = new ValueConjunction() { Sign = sign, Destination = value };
        return value;
    }
}