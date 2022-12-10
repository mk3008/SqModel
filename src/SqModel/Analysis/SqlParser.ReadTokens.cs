using SqModel.Extension;
using System.Text;

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
                CurrentToken = ReadWordUntilCloseBracket();
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
                CurrentToken = ReadWordUntilCloseBracket();
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
            else if (token == "--") token += ReadWordCrLfEnd();
            else if (token == "/*") token += ReadWordBlockCommentEnd();
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


    private string ReadWordUntilCloseBracket()
    {
        var s = new StringBuilder();

        var level = 1;

        while (true)
        {
            var c = PeekOrDefault();
            if (c == null) break;
            if (c == ')' && level == 1) break;

            var w = ReadWord();
            if (w.IsEmpty()) break;
            if (w == "--")
            {
                ReadWordCrLfEnd();
                continue;
            }
            if (w == "/*")
            {
                ReadWordBlockCommentEnd();
                continue;
            }
            if (w == "(") level++;
            if (w == ")") level--;
            s.Append(w);
        };
        return s.ToString();
    }

    private string ReadWordCrLfEnd()
    {
        var s = new StringBuilder();
        var ignoreSpaceTokens = "\r\n".ToCharArray();
        var w = ReadWord(ignoreSpaceTokens);

        while (w.IsNotEmpty())
        {
            s.Append(w);
            if (w == "\r" || w == "\r\n" || w == "\n") break;
            w = ReadWord(ignoreSpaceTokens);
        }
        return s.ToString();
    }

    private string ReadWordBlockCommentEnd()
    {
        var s = new StringBuilder();
        var level = 1;

        while (level != 0)
        {
            var w = ReadWord();
            s.Append(w);
            if (w.IsEmpty()) break;
            if (w == "/*") level++;
            if (w == "*/") level--;
        };
        return s.ToString();
    }

    public string ReadWord(char[]? ignoreSpaceTokens = null)
    {
        var cache = new StringBuilder();

        var nextchar = PeekOrDefault();
        if (nextchar == null) return cache.ToString();

        if (nextchar == '(' || nextchar == ')')
        {
            cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar == '/')
        {
            cache.Append(Read());
            nextchar = PeekOrDefault();
            if (nextchar == '*') cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar == '*')
        {
            cache.Append(Read());
            nextchar = PeekOrDefault();
            if (nextchar == '/') cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar == '-')
        {
            cache.Append(Read());
            nextchar = PeekOrDefault();
            if (nextchar == '-') cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar == '!')
        {
            cache.Append(Read());
            nextchar = PeekOrDefault();
            if (nextchar == '=') cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar == '>')
        {
            cache.Append(Read());
            nextchar = PeekOrDefault();
            if (nextchar == '=') cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar == '<')
        {
            cache.Append(Read());
            nextchar = PeekOrDefault();
            if (nextchar == '=' || nextchar == '>') cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar == '\n')
        {
            cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar == '\r')
        {
            cache.Append(Read());
            nextchar = PeekOrDefault();
            if (nextchar == '\n') cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar.IsSymbol())
        {
            cache.Append(Read());
            return cache.ToString();
        }

        if (nextchar.IsSpace())
        {
            while (nextchar.IsSpace() && !(ignoreSpaceTokens.Contains(nextchar)))
            {
                cache.Append(Read());
                nextchar = PeekOrDefault();
            }
            return cache.ToString();
        };

        if (nextchar.IsNumeric())
        {
            var hasdot = false;
            while (nextchar.IsNumeric() || (hasdot == false && nextchar == '.'))
            {
                if (nextchar == '.') hasdot = true;
                cache.Append(Read());
                nextchar = PeekOrDefault();
            }
            return cache.ToString();
        };

        while (!nextchar.IsSymbol() && !nextchar.IsSpace() && nextchar != null)
        {
            cache.Append(Read());
            nextchar = PeekOrDefault();
        }
        return cache.ToString();
    }
}