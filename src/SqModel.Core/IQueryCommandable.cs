namespace SqModel.Core;

public interface IQueryCommandable : IQueryCommand, IQueryParameter
{
    QueryCommand ToCommand();
}