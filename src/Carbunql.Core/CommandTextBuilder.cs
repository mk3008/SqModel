using Carbunql.Core.Extensions;
using Carbunql.Core.Tables;
using Carbunql.Core.Values;
using Cysharp.Text;
using System;
using System.Reflection.Emit;

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

    public int Level { get; set; }

    public string Indent { get; set; } = string.Empty;

    public List<(Token Token, int Level)> TokenIndents { get; set; } = new();

    private Utf16ValueStringBuilder sb = ZString.CreateStringBuilder();

    public string Execute(IEnumerable<Token> tokens)
    {
        var isFirst = true;

        foreach (var t in tokens)
        {
            if (isFirst)
            {
                isFirst = false;
            }

            if (PrevToken == null || t.Parents().Count() == PrevToken.Parents().Count())
            {
                WriteToken(t);
                continue;
            }

            if (t.Parents().Count() > PrevToken.Parents().Count())
            {
                if (t.Parent != null && Formatter.IsIncrementIndentOnBeforeWriteToken(t.Parent))
                {
                    Level++;
                    TokenIndents.Add((t.Parent, Level));
                    DoLineBreak();
                }
                WriteToken(t);
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
                DoLineBreak();
            }
            WriteToken(t);
        }
        return sb.ToString();
    }

    private string GetTokenText(Token token)
    {
        var isAppendSplitter = () =>
        {
            if (PrevToken == null) return false;

            if (PrevToken!.Text == "(") return false;
            if (PrevToken!.Text == ".") return false;

            if (token.Text.StartsWith("::")) return false;
            if (token.Text == ")") return false;
            if (token.Text == ",") return false;
            if (token.Text == ".") return false;
            if (token.Text == "(")
            {
                if (token.Sender is VirtualTable) return true;
                if (token.Sender is FunctionValue) return false;
                return true;
            }

            return true;
        };

        var s = isAppendSplitter() ? " " : "";
        PrevToken = token;
        if (token.IsReserved) return s + token.Text.ToUpper();
        return s + token.Text;
    }

    private void WriteToken(Token? token)
    {
        if (token == null) return;
        if (string.IsNullOrEmpty(token.Text)) return;

        if (PrevToken != null && Formatter.IsLineBreakOnBeforeWriteToken(token))
        {
            DoLineBreak();
        }
        sb.Append(GetTokenText(token));
        if (Formatter.IsLineBreakOnAfterWriteToken(token))
        {
            DoLineBreak();
        }
    }

    private void DoLineBreak()
    {
        PrevToken = null;
        Indent = (Level * 4).ToSpaceString();
        sb.Append("\r\n" + Indent);
    }
}