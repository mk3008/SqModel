//using Cysharp.Text;

//namespace Carbunql.Core;

//public class CommandTextBuilder
//{
//    public CommandTextBuilder(CommandFormatter formatter)
//    {
//        Formatter = formatter;
//    }

//    public CommandFormatter Formatter { get; init; }

//    public string Execute(IEnumerable<Token> tokens)
//    {
//        var sb = ZString.CreateStringBuilder();

//        Formatter.OnStart();

//        foreach (var t in tokens)
//        {
//            if (t.block != BlockType.Start) break;
//            WriteBlock(t, ref tokens, ref sb);
//        }

//        Formatter.OnEnd();

//        return sb.ToString();
//    }

//    private Token WriteBlock(Token blockstart, ref IEnumerable<Token> tokens, ref Utf16ValueStringBuilder sb)
//    {
//        var s = blockstart;
//        sb.Append(Formatter.OnStartBlock(s.type, s.text));

//        var e = WriteItem(s, s, ref tokens, ref sb);

//        sb.Append(Formatter.OnEndBlock(s.type, s.text));
//        return e;
//    }

//    private (TokenType type, BlockType block, string text) WriteItem((TokenType type, BlockType block, string text) owner,
//        (TokenType type, BlockType block, string text) group,
//        ref IEnumerable<(TokenType type, BlockType block, string text)> tokens,
//        ref Utf16ValueStringBuilder sb)
//    {
//        WriteStartBlockItem(group, ref sb);

//        foreach (var t in tokens)
//        {
//            if (t.block == BlockType.Split)
//            {
//                WriteEndBlockItem(group, ref sb);
//                return WriteItem(owner, t, ref tokens, ref sb);
//            }

//            if (t.block == BlockType.End)
//            {
//                WriteEndBlockItem(group, ref sb);
//                return t;
//            }

//            if (t.block == BlockType.Start)
//            {
//                // nested block
//                return WriteBlock(t, ref tokens, ref sb);
//            }

//            WriteToken(t, ref sb);
//        }
//        throw new InvalidOperationException();
//    }

//    private void WriteStartBlockItem(Token token, ref Utf16ValueStringBuilder sb)
//    {
//        sb.Append(Formatter.OnStartItemBeforeWriteToken(token.type, token.text));
//        WriteToken(token, ref sb);
//        sb.Append(Formatter.OnStartItemAfterWriteToken(token.type, token.text));
//    }

//    private void WriteEndBlockItem(Token token, ref Utf16ValueStringBuilder sb)
//    {
//        sb.Append(Formatter.OnEndItemBeforeWriteToken(token.type, token.text));
//        WriteToken(token, ref sb);
//        sb.Append(Formatter.OnEndItemAfterWriteToken(token.type, token.text));
//    }

//    private void WriteToken(Token token, ref Utf16ValueStringBuilder sb)
//    {
//        if (!string.IsNullOrEmpty(token.text))
//        {
//            sb.Append(Formatter.OnBeforeWriteToken(token.type, token.block, token.text));
//            sb.Append(Formatter.WriteToken(token.type, token.block, token.text));
//            sb.Append(Formatter.OnAfterWriteToken(token.type, token.block, token.text));
//        }
//    }
//}