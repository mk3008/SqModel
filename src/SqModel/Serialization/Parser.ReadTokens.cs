using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public string CurrentToken { get; private set; } = string.Empty;

    public IEnumerable<string> ReadTokensWithoutComment(bool includeCurrentToken)
    {
        var tokens = (includeCurrentToken && !string.IsNullOrEmpty(CurrentToken)) ? new[] { CurrentToken } : Enumerable.Empty<string>();
        return tokens.Union(ReadTokens()).Where(x => !x.StartsWith("--") && !x.StartsWith("/*"));
    }

    public IEnumerable<string> ReadTokens()
    {
        while (true)
        {
            var token = ReadToken();
            if (string.IsNullOrEmpty(token)) break;

            if (token == "(")
            {
                yield return token;
                token = ReadUntilCloseBracket();
            }
            yield return token;
        }
    }

    public string ReadToken()
    {
        ReadWhileSpace();

        var read = () =>
        {
            var token = ReadWord();

            if (string.IsNullOrEmpty(token)) return string.Empty;

            if (token == "'") token += ReadWhileQuoteToken();
            else if (token == "--") token += ReadUntilCrLf();
            else if (token == "/*") token += ReadWhileBlockCommentToken();
            else if (token.ToLower() == "inner" || token.ToLower() == "cross")
            {
                ReadWhileSpace();
                var next = ReadWord();
                if (next.ToLower() != "join") throw new SyntaxException(token.ToLower());
                token += " " + next;
            }
            else if (token.ToLower() == "left" || token.ToLower() == "right")
            {
                ReadWhileSpace();
                var next = ReadWord();
                if (next.ToLower() == "outer")
                {
                    token += " " + next;
                    ReadWhileSpace();
                    next = ReadWord();
                }
                if (next.ToLower() != "join") throw new SyntaxException(token.ToLower());
                token += " " + next;
            }
            else if (token.ToLower() == "order" || token.ToLower() == "group")
            {
                ReadWhileSpace();
                var next = ReadWord();
                if (next.ToLower() != "by") throw new SyntaxException(token.ToLower());
                token += " " + next;
            }
            return token;
        };

        CurrentToken = read();
        return CurrentToken;
    }

    public string ReadWord()
    {
        var cache = new StringBuilder();
        var isSymbolToken = false;

        var cn = PeekOrDefault();
        while (cn != null)
        {
            if (isSymbolToken || cn.IsSymbol() && cache.Length == 0)
            {
                isSymbolToken = true;
                cache.Append(Read());
                if (cache.Length == 1 && "()".Any(cache.ToString().First())) break;
                if (cache.Length == 2 && (cache.ToString() == "--" || cache.ToString() == "/*")) break;
                cn = PeekOrDefault();
                if (!cn.IsSymbol()) break;
                continue;
            }

            cache.Append(Read());

            cn = PeekOrDefault();
            if (cn.IsSymbol() || cn.IsSpace()) break;
        }

        return cache.ToString();
    }
}