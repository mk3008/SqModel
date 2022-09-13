using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Analysis;
using SqModel.Extension;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public string CurrentToken { get; private set; } = string.Empty;

    public IEnumerable<string> ReadTokensWithoutComment()
        => ReadTokens().Where(x => !x.StartsWith("--") && !x.StartsWith("/*"));

    public IEnumerable<string> ReadTokens()
    {
        while (true)
        {
            if (CurrentToken == "(")
            {
                CurrentToken = ReadUntilCloseBracket();
                yield return CurrentToken;
                continue;
            }

            var token = ReadToken();
            if (string.IsNullOrEmpty(token))
            {
                yield return String.Empty;
                break;
            };

            if (token == "(")
            {
                yield return token;
                CurrentToken = ReadUntilCloseBracket();
                yield return CurrentToken;
                continue;
            }

            yield return token;
        }
    }

    public string ReadToken(bool isSkipSpaceToken = true)
    {
        if (isSkipSpaceToken) ReadWhileSpace();

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
            if (cn.IsSpace()) break;

            cn = PeekOrDefault();
            if (cn.IsSymbol() || cn.IsSpace()) break;
        }

        return cache.ToString();
    }
}