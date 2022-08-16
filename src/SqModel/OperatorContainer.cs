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

    public bool IsRoot { get; set; } = false;

    public ValueContainer? Condition { get; set; } = null;

    public List<OperatorContainer>? ConditionGroup { get; set; } = null;

    public OperatorContainer And => SetOperator("and");

    public OperatorContainer Or => SetOperator("or");

    public OperatorContainer Not => AddOperator("not");

    internal OperatorContainer SetOperator(string @operator)
    {
        Operator = @operator;
        return this;
    }

    internal OperatorContainer AddOperator(string @operator)
    {
        Operator = $"{Operator} {@operator}";
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
            var q = new Query();
            var cnt = 0;
            ConditionGroup.ForEach(x =>
            {
                cnt++;
                if (cnt == 1)
                {
                    q = x.ToQuery();
                }
                else
                {
                    q = q.Merge(x.ToQuery().InsertToken(x.Operator), Splitter);
                }
            });
            if (!IsRoot && cnt != 1) q.DecorateBracket();
            return q;
        }
        return new Query();
    }
}
