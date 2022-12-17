using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core;


public abstract class CommandFormatter
{
    public abstract void OnStart();

    public abstract string OnStartBlock(TokenType type, string text);

    public abstract string OnEndItemBeforeWriteToken(TokenType type, string text);

    public abstract string OnStartItemBeforeWriteToken(TokenType type, string text);

    public abstract string OnBeforeWriteToken(TokenType type, BlockType block, string text);

    public abstract string WriteToken(TokenType type, BlockType block, string text);

    public abstract string OnAfterWriteToken(TokenType type, BlockType block, string text);

    public abstract string OnStartItemAfterWriteToken(TokenType type, string text);

    public abstract string OnEndItemAfterWriteToken(TokenType type, string text);

    public abstract string OnEndBlock(TokenType type, string text);

    public abstract void OnEnd();
}