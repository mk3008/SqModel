namespace SqModel.Core;

public interface IQueryable
{
    string GetCommandText();

    IDictionary<string, object?> GetParameters();
}