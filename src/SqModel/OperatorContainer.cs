using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class OperatorContainer
{
    public string Operator { get; set; } = "and";

    public string SubOperator { get; set; } = "";

    public bool IsRoot { get; set; } = false;

    public ValueContainer? Condition { get; set; } = null;

    public List<OperatorContainer>? ConditionGroup { get; set; } = null;

    public OperatorContainer And => SetOperator("and");

    public OperatorContainer Or => SetOperator("or");

    public OperatorContainer Not => SetSubOperator("not");

    internal OperatorContainer SetOperator(string @operator)
    {
        Operator = @operator;
        SubOperator = String.Empty;
        return this;
    }

    internal OperatorContainer SetSubOperator(string @operator)
    {
        SubOperator = @operator;
        return this;
    }

    internal OperatorContainer SetOperator(string @operator, string suboperator)
    {
        Operator = @operator;
        SubOperator = suboperator;
        return this;
    }

    public OperatorContainer Where() => GetNewOperatorContainer();

    public OperatorContainer GetNewOperatorContainer()
    {
        var c = new OperatorContainer();
        Add(c);
        return c;
    }

    public void Add(OperatorContainer container)
    {
        ConditionGroup ??= new();
        ConditionGroup.Add(container);
    }

    public string Splitter { get; set; } = " ";

    public Query ToQuery()
    {
        if (Condition != null) return Condition.ToQuery();
        if (ConditionGroup != null)
        {
            var fn = (OperatorContainer x) => x.ToQuery().InsertToken(x.SubOperator);

            var q = new Query();
            var cnt = 0;
            ConditionGroup.ForEach(x =>
            {
                cnt++;
                if (cnt == 1)
                {
                    q = fn(x);
                    return;
                }
                q = q.Merge(fn(x).InsertToken(x.Operator), Splitter);
            });
            if (!IsRoot && cnt != 1) q.DecorateBracket();
            return q;
        }
        return new Query();
    }
}
