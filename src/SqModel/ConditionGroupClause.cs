using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ConditionGroupClause
{
    public string Operator { get; set; } = String.Empty;

    public List<ConditionClause> Conditions { get; set; } = new();

    public List<ConditionGroupClause> GroupConditions { get; set; } = new();

    public string Splitter { get; set; } = " ";

    public Query ToQuery()
    {
        var subCount = 0;
        var totalCount = 0;

        var q = new Query();

        Conditions.ForEach(x =>
        {
            subCount++;
            totalCount++;
            if (subCount == 1)
            {
                q = x.ToQuery();
            }
            else
            {
                q = q.Merge(x.ToQuery().InsertToken(x.Operator), Splitter);
            }
        });

        subCount = 0;
        GroupConditions.ForEach(x =>
        {
            subCount++;
            totalCount++;
            if (subCount == 1 && totalCount == 1)
            {
                q = x.ToQuery().DecorateBracket();
            }
            else
            {
                q = q.Merge(x.ToQuery().DecorateBracket().InsertToken(x.Operator), Splitter);
            }
        });

        return q;
    }
}
