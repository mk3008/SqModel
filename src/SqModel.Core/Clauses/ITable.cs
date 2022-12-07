namespace SqModel.Core.Clauses;

public interface ITable : IQueryCommand, IQueryParameter
{
    string GetDefaultName();
}