using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public IEnumerable<string> ReadAllTokens()
    {
        while (true)
        {
            ReadWhileSpace();
            var token = ReadToken();

            if (string.IsNullOrEmpty(token)) break;

            if (token == "'")
            {
                token += ReadWhileQuoteToken();
            }
            else if (token == "(")
            {
                yield return token;
                token = ReadUntilCloseBracket();
            }
            else if (token == "--")
            {
                token += ReadUntilCrLf();
            }
            else if (token == "/*")
            {
                token += ReadWhileBlockCommentToken();
            }
            else if (token.ToLower() == "inner" || token.ToLower() == "cross")
            {
                ReadWhileSpace();
                var next = ReadToken();
                if (next.ToLower() != "join") throw new SyntaxException(token.ToLower());
                token += " " + next;
            }
            else if (token.ToLower() == "left" || token.ToLower() == "right")
            {
                ReadWhileSpace();
                var next = ReadToken();
                if (next.ToLower() == "outer")
                {
                    token += " " + next;
                    ReadWhileSpace();
                    next = ReadToken();
                }
                if (next.ToLower() != "join") throw new SyntaxException(token.ToLower());
                token += " " + next;
            }
            else if (token.ToLower() == "order" || token.ToLower() == "group")
            {
                ReadWhileSpace();
                var next = ReadToken();
                if (next.ToLower() != "by") throw new SyntaxException(token.ToLower());
                token += " " + next;
            }
            yield return token;
        }
    }

    public string ReadToken()
    {
        var tokens = CommandTokens.Union(SymbolTokens.Select(x => x.ToString())).Union(SpaceTokens.Select(x => x.ToString())).Union(new[] { LineCommentToken, BlockCommentToken });

        Logger?.Invoke(">start ReadToken");

        var cache = new StringBuilder();

        var isPeekChar = (char c) =>
        {
            var nextc = PeekOrDefault();
            return (nextc != null && nextc.Value == c);
        };

        while (true)
        {
            var cn = PeekOrDefault();
            if (cn == null) break;

            if (cn.IsSymbol())
            {
                if (cache.Length != 0) break;

                cache.Append(Read());
                if (
                    (cn.Value == '-' && isPeekChar('-'))
                    || (cn.Value == '/' && isPeekChar('*'))
                    ) cache.Append(Read());
                break;
            }

            if (cn.IsSpace()) break;

            cache.Append(Read());
            Logger!.Invoke($"cache:{cache}");

            continue;
        }

        Logger!.Invoke($"result Token:{cache}");
        Logger?.Invoke(">end   ReadToken");

        return cache.ToString();
    }
}