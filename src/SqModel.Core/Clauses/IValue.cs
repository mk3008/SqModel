namespace SqModel.Core.Clauses;

public interface IValue : IQueryCommand
{
    string GetDefaultName();
}