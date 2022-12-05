namespace SqModel.Core.Clauses;

public interface ITable : IQueryCommand
{
    string GetDefaultName();
}