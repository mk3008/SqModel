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
    public bool DoSplitBefore { get; set; } = true;

    private bool DoSplitAfter => !DoSplitBefore;

    public bool UseUpperCaseReservedToken { get; set; } = true;

    public bool AdjustFirstLineIndex { get; set; } = true;

    public bool DoIndentInsideBracket { get; set; } = false;

    private string Indent { get; set; } = string.Empty;

    private int IndentLevel { get; set; } = 0;

    private int BracketLevel { get; set; } = 0;

    private bool IsNewLine { get; set; } = false;

    private (TokenType type, BlockType block, string text)? prevToken { get; set; }

    public string ToString(IEnumerable<(TokenType type, BlockType block, string text)> tokens)
    {
        var sb = ZString.CreateStringBuilder();
        var isFirst = true;
        prevToken = null;

        foreach (var token in tokens)
        {
            UpdateIsNewLineOnBefore(token);
            UpdateIndentLevelOnBefore(token);

            if (isFirst)
            {
                isFirst = false;
            }
            else if (IsNewLine)
            {
                sb.Append("\r\n" + Indent);
                if (token.block != BlockType.Splitter && AdjustFirstLineIndex && DoSplitBefore) sb.Append("  ");
            }
            else if (!IsNewLine)
            {
                if (prevToken == null || prevToken.Value.text != "(")
                {
                    if (token.text != "," && token.text != ")")
                    {
                        sb.Append(" ");
                    }
                }
            }

            sb.Append(GetText(token));

            UpdateBracketLevel(token);
            UpdateIsNewLineOnAfter(token);
            UpdateIndentLevelOnAfter(token);

            prevToken = token;
        }

        return sb.ToString();
    }

    private string GetText((TokenType type, BlockType block, string text) token)
    {
        if (UseUpperCaseReservedToken && token.type == TokenType.Reserved) return token.text.ToUpper();
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
            if (DoSplitBefore)
            {
                if (BracketLevel == 0 || DoIndentInsideBracket)
                {
                    IsNewLine = true;
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
            }
            else
            {
                IsNewLine = false;
            }
            return;
        }

        if (token.block == BlockType.Splitter)
        {
            if (DoSplitAfter)
            {
                if (BracketLevel == 0 || DoIndentInsideBracket)
                {
                    IsNewLine = true;
                }
                else
                {
                    IsNewLine = false;
                }
            }
            else
            {
                IsNewLine = false;
            }
            return;
        }

        IsNewLine = false;
    }


    private void UpdateBracketLevel((TokenType type, BlockType block, string text) token)
    {
        if (token.text == "(")
        {
            BracketLevel++;
        }
        else if (token.text == ")")
        {
            BracketLevel--;
        }
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
}