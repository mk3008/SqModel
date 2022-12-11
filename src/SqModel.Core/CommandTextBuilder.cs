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

    private string Indent { get; set; } = string.Empty;

    private int IndentLevel { get; set; } = 0;

    private int BracketLevel { get; set; } = 0;

    private bool IsNewLine { get; set; } = false;

    private string ClauseName { get; set; } = string.Empty;

    private (TokenType type, BlockType block, string text)? PrevToken { get; set; }

    public string Execute(IEnumerable<(TokenType type, BlockType block, string text)> tokens)
    {
        var sb = ZString.CreateStringBuilder();
        var isFirst = true;
        PrevToken = null;

        foreach (var token in tokens)
        {
            if (token.type == TokenType.Clause) ClauseName = token.text;

            UpdateIsNewLineOnBefore(token);
            UpdateBracketLevelOnBefore(token);
            UpdateIndentLevelOnBefore(token);

            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                AppendIndent(token, ref sb);
            }

            if (token.type != TokenType.Control) sb.Append(GetText(token));

            UpdateBracketLevelOnAfter(token);
            UpdateIsNewLineOnAfter(token);
            UpdateIndentLevelOnAfter(token);

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
            if (BracketLevel != 0) return;
            if (AdjustFirstLineIndent && DoSplitBefore && ClauseName.AreEqual("select")) sb.Append("  ");
            return;
        }
        else if (!IsNewLine)
        {
            if (PrevToken == null) return;
            if (PrevToken.Value.text == "(") return;
            if (token.text != "," && token.text != ")") sb.Append(" ");
            return;
        }
    }

    private string GetText((TokenType type, BlockType block, string text) token)
    {
        if (!UseUpperCaseReservedToken) return token.text;
        if (token.type == TokenType.Reserved || token.type == TokenType.Clause) return token.text.ToUpper();
        return token.text;
    }

    private void UpdateIsNewLineOnBefore((TokenType type, BlockType block, string text) token)
    {
        if (token.block == BlockType.BlockEnd)
        {
            if (BracketLevel == 0 || DoIndentInsideBracket)
            {
                IsNewLine = true;
            }
            return;
        }

        if (token.block == BlockType.Splitter)
        {
            if (BracketLevel == 0 || DoIndentInsideBracket)
            {
                if (token.text != ",")
                {
                    IsNewLine = true;
                    return;
                }
                else if (token.text == "," && DoSplitBefore)
                {
                    IsNewLine = true;
                    return;
                }
            }
            return;
        }

        return;
    }

    private void UpdateIsNewLineOnAfter((TokenType type, BlockType block, string text) token)
    {
        if (token.block == BlockType.BlockStart)
        {
            if (BracketLevel == 0 || DoIndentInsideBracket)
            {
                IsNewLine = true;
                return;
            }
            IsNewLine = false;
            return;
        }

        if (token.block == BlockType.Splitter)
        {
            if (BracketLevel == 0 || DoIndentInsideBracket)
            {
                if (token.text != ",")
                {
                    IsNewLine = true;
                    return;
                }
                else if (token.text == "," && DoSplitAfter)
                {
                    IsNewLine = true;
                    return;
                }
            }
            IsNewLine = false;
            return;
        }

        if (token.type == TokenType.Control) return;

        IsNewLine = false;
    }

    private void UpdateBracketLevelOnBefore((TokenType type, BlockType block, string text) token)
    {
        if (token.text == ")") BracketLevel--;
    }

    private void UpdateBracketLevelOnAfter((TokenType type, BlockType block, string text) token)
    {
        if (token.text == "(") BracketLevel++;
    }

    private void UpdateIndentLevelOnBefore((TokenType type, BlockType block, string text) token)
    {
        if (!IsNewLine) return;
        if (token.block == BlockType.BlockEnd)
        {
            IndentLevel--;
            Indent = (IndentLevel * 4).ToSpaceString();
        }
    }

    private void UpdateIndentLevelOnAfter((TokenType type, BlockType block, string text) token)
    {
        if (!IsNewLine) return;
        if (token.block == BlockType.BlockStart)
        {
            IndentLevel++;
            Indent = (IndentLevel * 4).ToSpaceString();
        }
    }

    private void Reset()
    {
        IndentLevel = 0;
        BracketLevel = 0;
        Indent = string.Empty;
        PrevToken = null;
        IsNewLine = false;
        ClauseName = string.Empty;
    }
}