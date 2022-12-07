namespace SqModel.Core;

public interface IQueryable : IQueryCommand, IQueryParameter
{
    Query ToQuery();
}