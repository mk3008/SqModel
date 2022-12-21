using Cysharp.Text;
using System.Linq;

namespace Carbunql.Core;

public class CommandTextBuilder
{
    public CommandTextBuilder(CommandFormatter formatter)
    {
        Formatter = formatter;
    }

    public CommandTextBuilder()
    {
        Formatter = new CommandFormatter();
    }

    public CommandFormatter Formatter { get; init; }

    public string Execute(IEnumerable<Token> tokens)
    {
        var sb = ZString.CreateStringBuilder();



        Token? prev = null;
        var isFirst = true;


        foreach (var t in tokens)
        {
            if (isFirst)
            {
                Formatter.OnStart(t);
                isFirst = false;
            }

            // || t.Sender.Equals(prev.Sender)
            if (prev == null || t.Parents().Count() == prev.Parents().Count())
            {
                WriteToken(t, ref sb);
            }
            else if (t.Parents().Count() > prev.Parents().Count())
            {
                sb.Append(Formatter.OnStartBlock(t));
                WriteToken(t, ref sb);
            }
            else
            {
                sb.Append(Formatter.OnEndBlockBeforeWriteToken(t));
                WriteToken(t, ref sb);
                sb.Append(Formatter.OnEndBlockAfterWriteToken(t));
            }
            prev = t;
        }

        Formatter.OnEnd();

        return sb.ToString();
    }

    private void WriteToken(Token? token, ref Utf16ValueStringBuilder sb)
    {
        if (token == null) return;
        if (string.IsNullOrEmpty(token.Text)) return;
        sb.Append(Formatter.OnBeforeWriteToken(token));
        sb.Append(Formatter.WriteToken(token));
        sb.Append(Formatter.OnAfterWriteToken(token));
    }
}