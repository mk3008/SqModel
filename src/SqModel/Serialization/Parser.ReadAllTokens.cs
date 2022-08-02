using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public string CurrentToken { get; private set; } = string.Empty;

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
                CurrentToken = token;
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

            CurrentToken = token;
            yield return token;
        }
    }

    public string ReadToken()
    {
        //Logger?.Invoke(">start ReadToken");

        var cache = new StringBuilder();
        var isSymbolToken = false;

        var cn = PeekOrDefault();
        while (cn != null)
        {
            if (isSymbolToken || cn.IsSymbol() && cache.Length == 0)
            {
                isSymbolToken = true;
                cache.Append(Read());
                if (cache.Length == 2 && (cache.ToString() == "--" || cache.ToString() == "/*")) break;
                cn = PeekOrDefault();
                if (!cn.IsSymbol()) break;
                continue;
            }

            cache.Append(Read());

            cn = PeekOrDefault();
            if (cn.IsSymbol() || cn.IsSpace()) break;
        }

        //Logger!.Invoke($"result Token:{cache}");
        //Logger?.Invoke(">end   ReadToken");

        return cache.ToString();
    }
}