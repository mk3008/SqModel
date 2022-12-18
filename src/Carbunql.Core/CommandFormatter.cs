namespace Carbunql.Core;


public abstract class CommandFormatter
{
    public abstract void OnStart();

    public abstract string OnStartBlock(Token token);

    public abstract string OnEndItemBeforeWriteToken(Token token);

    public abstract string OnStartItemBeforeWriteToken(Token token);

    public abstract string OnBeforeWriteToken(Token token);

    public abstract string WriteToken(Token token);

    public abstract string OnAfterWriteToken(Token token);

    public abstract string OnStartItemAfterWriteToken(Token token);

    public abstract string OnEndItemAfterWriteToken(Token token);

    public abstract string OnEndBlock(Token token);

    public abstract void OnEnd();
}