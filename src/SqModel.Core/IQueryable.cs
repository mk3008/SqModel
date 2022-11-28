namespace SqModel.Core;

public interface IQueryable : IQueryCommand
{
    IDictionary<string, object?> GetParameters();
}