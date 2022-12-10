namespace SqModel;

public interface ICondition : IQueryable
{
    string Operator { get; set; }

    string SubOperator { get; set; }
}