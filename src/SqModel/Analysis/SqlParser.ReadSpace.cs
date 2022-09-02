using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Extension;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public string ReadUntilTokens(List<string> breakTokens, string leveluptoken = "", string leveldowntoken = "")
    {
        var sb = new StringBuilder();

        var level = 0;
        while (true)
        {
            var token = ReadToken(false);
            if (token == " ")
            {
                sb.Append(token);
                continue;
            }
            if (token.IsEmpty()) break;

            if (token.ToLower() == leveluptoken) level++;
            if (level == 0 && breakTokens.Contains(token.ToLower())) break;

            if (token.ToLower() == leveldowntoken) level--;
            sb.Append(token);
        }
        return sb.ToString();
    }

    public string ReadUntilCrLf()
    {
        var digit = (char c) =>
        {
            if (c == '\n')
            {
                Read();
                return false;
            }
            if (c == '\r')
            {
                Read();
                var next = PeekOrDefault();
                if (next.HasValue && next.Value == '\n')
                {
                    Read();
                    return false;
                }
                return false;
            }
            return true;
        };
        return ReadWhile(digit);
    }

    public string ReadWhileSpace()
    {
        var digit = (char c) => c.IsSpace();
        return ReadWhile(digit);
    }

    public string ReadUntilSpaceOrSymbol()
    {
        var digit = (char c) => !c.IsSpace() && !c.IsSymbol();
        return ReadWhile(digit);
    }

    public string ReadWhile(Func<char, bool> digit)
    {
        var s = new StringBuilder();

        while (true)
        {
            var c = PeekOrDefault();
            if (c == null) break;
            if (!digit(c.Value)) break;
            s.Append(Read());
            continue;
        };

        return s.ToString();
    }

    public string ReadUntilCloseBracket()
    {
        var s = new StringBuilder();

        var level = 1;

        while (level != 0)
        {
            var c = PeekOrDefault();
            if (c == null) break;

            if (c == '(') level++;
            if (c == ')') level--;

            if (level != 0) s.Append(Read());
        };
        return s.ToString();
    }

    public string ReadWhileBlockCommentToken()
    {
        var s = new StringBuilder();

        var level = 1;

        while (level != 0)
        {

            var c = PeekOrDefault();
            if (c == null) break;

            if (c != '/' && c != '*')
            {
                s.Append(Read());
                continue;
            }

            var token = Read().ToString();
            c = PeekOrDefault();
            if (c.HasValue)
            {
                token += Read().ToString();
                if (token == "/*") level++;
                if (token == "*/") level--;
            }
            s.Append(token);
        };
        return s.ToString();
    }

    public string ReadWhileQuoteToken()
    {
        var s = new StringBuilder();

        while (true)
        {
            var c = PeekOrDefault();
            if (c == null) break;

            if (c != '\'')
            {
                s.Append(Read());
                continue;
            }

            s.Append(Read());
            c = PeekOrDefault();
            if (c.HasValue && c == '\'')
            {
                //espaced
                s.Append(Read());
                continue;
            }
            break;
        };
        return s.ToString();
    }
}