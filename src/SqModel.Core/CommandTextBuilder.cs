using Cysharp.Text;
using SqModel.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core;

public class CommandTextBuilder
{
    public bool DoSplitBefore { get; set; } = false;

    private bool DoSplitAfter => !DoSplitBefore;

    public bool UseUpperCaseReservedToken { get; set; } = true;

    public bool AdjustFirstLineIndent { get; set; } = true;

    public bool DoIndentInsideBracket { get; set; } = false;

    public bool DoIndentJoinCondition { get; set; } = false;

    private string Indent { get; set; } = string.Empty;

    private int IndentLevel { get; set; } = 0;

    private bool IsNewLine { get; set; } = false;

    private List<string> Blocks { get; set; } = new();

    private string GetCurrentBlock()
    {
        if (!Blocks.Any()) return string.Empty;
        return Blocks[Blocks.Count - 1];
    }

    private (TokenType type, BlockType block, string text)? PrevToken { get; set; }

    public string Execute(IEnumerable<(TokenType type, BlockType block, string text)> tokens)
    {
        var sb = ZString.CreateStringBuilder();
        var isFirst = true;
        PrevToken = null;

        foreach (var token in tokens)
        {
            UpdateStatus(token);

            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                AppendIndent(token, ref sb);
            }

            if (token.type != TokenType.Control) sb.Append(GetText(token));

            PrevToken = token;
        }

        Reset();
        return sb.ToString();
    }

    private void AppendIndent((TokenType type, BlockType block, string text) token, ref Utf16ValueStringBuilder sb)
    {
        if (token.type == TokenType.Control) return;

        if (IsNewLine)
        {
            sb.Append("\r\n" + Indent);
            if (IndentLevel == 0) return;
            if (token.block == BlockType.Splitter) return;
            if (AdjustFirstLineIndent && DoSplitBefore && GetCurrentBlock().AreEqual("select")) sb.Append("  ");
            return;
        }
        else if (!IsNewLine)
        {
            if (PrevToken == null) return;
            if (PrevToken.Value.text == "(") return;
            if (PrevToken.Value.text != "," && token.text == "(") return;
            if (token.text != "," && token.text != ")") sb.Append(" ");
            return;
        }
    }

    private string GetText((TokenType type, BlockType block, string text) token)
    {
        if (!UseUpperCaseReservedToken) return token.text;
        switch (token.type)
        {
            case TokenType.Reserved:
            case TokenType.Clause:
            case TokenType.Operator:
                return token.text.ToUpper();
        }
        return token.text;
    }

    private void UpdateStatus((TokenType type, BlockType block, string text) token)
    {
        var oldIndent = Indent;

        var add = (string name) =>
        {
            IndentLevel++;
            Blocks.Add(name);
            Indent = (IndentLevel * 4).ToSpaceString();
        };

        var remove = (string name) =>
        {
            IndentLevel--;
            Blocks.RemoveAt(Blocks.Count - 1);
            Indent = (IndentLevel * 4).ToSpaceString();
        };

        if (PrevToken != null && PrevToken.Value.block == BlockType.BlockStart)
        {
            var t = PrevToken.Value;
            var tokens = new string[] { "(", "on" };

            if (!t.text.AreContains(tokens))
            {
                add(t.text);
            }
            else if (t.text == "(" && DoIndentInsideBracket)
            {
                add(t.text);
            }
            else if (t.text.AreEqual("on") && DoIndentJoinCondition)
            {
                add(t.text);
            }
        }
        if (token.block == BlockType.BlockEnd)
        {
            var tokens = new string[] { ")", "on" };

            if (!token.text.AreContains(tokens))
            {
                remove(token.text);
            }
            else if (token.text == ")" && DoIndentInsideBracket)
            {
                remove(token.text);
            }
            else if (token.text.AreEqual("on") && DoIndentJoinCondition)
            {
                remove(token.text);
            }
        }

        if (oldIndent != Indent)
        {
            IsNewLine = true;
            return;
        };
        if (token.block == BlockType.Splitter && DoSplitBefore)
        {
            IsNewLine = true;
            return;
        };
        if (PrevToken != null)
        {
            var t = PrevToken.Value;
            if ((t.block == BlockType.BlockEnd || t.block == BlockType.Splitter) && t.type == TokenType.Control)
            {
                IsNewLine = true;
                return;
            }
            if (t.block == BlockType.Splitter && DoSplitAfter)
            {
                IsNewLine = true;
                return;
            }
        };
        IsNewLine = false;
        return;
    }

    private void Reset()
    {
        IndentLevel = 0;
        Indent = string.Empty;
        PrevToken = null;
        IsNewLine = false;
        Blocks.Clear();
    }
}