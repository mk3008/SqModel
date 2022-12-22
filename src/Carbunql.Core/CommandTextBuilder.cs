using Carbunql.Core.Extensions;
using Carbunql.Core.Tables;
using Carbunql.Core.Values;
using Cysharp.Text;
using System;
using System.Reflection.Emit;
using System.Text;

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

    private Token? PrevToken { get; set; }

    private int Level { get; set; }

    private string Indent { get; set; } = string.Empty;

    private List<(Token Token, int Level)> TokenIndents { get; set; } = new();

    public string Execute(IQueryCommand cmd)
    {
        return Execute(cmd.GetTokens(null));
    }

    public string Execute(IEnumerable<Token> tokens)
    {
        using var sb = ZString.CreateStringBuilder();

        foreach (var t in tokens)
        {
            if (PrevToken == null || t.Parents().Count() == PrevToken.Parents().Count())
            {
                foreach (var item in GetTokenTexts(t)) sb.Append(item);
                continue;
            }

            if (t.Parents().Count() > PrevToken.Parents().Count())
            {
                if (t.Parent != null && Formatter.IsIncrementIndentOnBeforeWriteToken(t.Parent))
                {
                    Level++;
                    TokenIndents.Add((t.Parent, Level));
                    sb.Append(GetLineBreakText());
                }
                foreach (var item in GetTokenTexts(t)) sb.Append(item);
                continue;
            }

            if (Formatter.IsDecrementIndentOnBeforeWriteToken(t))
            {
                var q = TokenIndents.Where(x => x.Token != null && x.Token.Equals(t.Parent)).Select(x => x.Level);
                if (q.Any())
                {
                    Level = q.First();
                }
                else
                {
                    Level = 0;
                }
                sb.Append(GetLineBreakText());
            }
            foreach (var item in GetTokenTexts(t)) sb.Append(item);
        }
        return sb.ToString();
    }

    private IEnumerable<string> GetTokenTexts(Token? token)
    {
        if (token == null) yield break;
        if (string.IsNullOrEmpty(token.Text)) yield break;

        if (PrevToken != null && Formatter.IsLineBreakOnBeforeWriteToken(token))
        {
            yield return GetLineBreakText();
        }
        yield return GetTokenTextCore(token);
        if (Formatter.IsLineBreakOnAfterWriteToken(token))
        {
            yield return GetLineBreakText();
        }
    }

    private string GetTokenTextCore(Token token)
    {
        using var sb = ZString.CreateStringBuilder();

        if (token.NeedsSpace(PrevToken)) sb.Append(' ');
        PrevToken = token;
        if (token.IsReserved)
        {
            sb.Append(token.Text.ToUpper());
        }
        else
        {
            sb.Append(token.Text);
        }

        return sb.ToString();
    }

    private string GetLineBreakText()
    {
        PrevToken = null;
        Indent = (Level * 4).ToSpaceString();
        return "\r\n" + Indent;
    }
}