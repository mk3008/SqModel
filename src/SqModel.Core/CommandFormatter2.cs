using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core;


//public class CommandFormatter : ICommandFormatter
//{
//    public bool DoSplitBefore { get; set; } = false;

//    //private bool DoSplitAfter => !DoSplitBefore;

//    //public bool UseUpperCaseReservedToken { get; set; } = true;

//    //public bool AdjustFirstLineIndent { get; set; } = true;

//    //public bool DoIndentInsideBracket { get; set; } = false;

//    //public bool DoIndentJoinCondition { get; set; } = false;

//    private string Indent { get; set; } = string.Empty;

//    private int IndentLevel { get; set; } = 0;

//    private bool IsNewLine { get; set; } = false;

//    private List<string> Blocks { get; set; } = new();

//    private string GetCurrentBlock()
//    {
//        if (!Blocks.Any()) return string.Empty;
//        return Blocks[Blocks.Count - 1];
//    }

//    public void OnStart()
//    {
//        Indent = string.Empty;
//        IndentLevel = 0;
//        IsNewLine = false;
//    }

//    public string OnBeforeBlock(TokenType tp, string text)
//    {
//        throw new NotImplementedException();
//    }

//    public string OnBeforeWriteToekn(TokenType tp, string text)
//    {
//        throw new NotImplementedException();
//    }

//    public string WriteToken(TokenType tp, string text)
//    {
//        throw new NotImplementedException();
//    }

//    public string OnAfterWriteToken(TokenType tp, string text)
//    {
//        throw new NotImplementedException();
//    }

//    public string OnAfterBlock(TokenType tp, string text)
//    {
//        throw new NotImplementedException();
//    }

//    public void OnEnd()
//    {
//        throw new NotImplementedException();
//    }

//    private (TokenType type, BlockType block, string text)? PrevToken { get; set; }


//}