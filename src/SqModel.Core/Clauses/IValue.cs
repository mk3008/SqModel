namespace SqModel.Core.Clauses;

public interface IValue
{
    string? GetName();

    string GetCommandText();

    NestedValue? Nest { get; set; }
}