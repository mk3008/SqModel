using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
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

    //public CloseTokenSet ReadUntilCloseToken(Func<string, DigitReturns> digit)
    //{
    //    var s = new StringBuilder();
    //    var cache = new StringBuilder();

    //    while (true)
    //    {
    //        var c = PeekOrDefault();
    //        if (c == null) break;

    //        var rtn = DigitReturns.Unknown;

    //        while (rtn != DigitReturns.Unknown)
    //        {
    //            c = PeekOrDefault();
    //            if (c == null)
    //            {
    //                rtn = DigitReturns.IsNotMatch;
    //            }
    //            else
    //            {
    //                cache.Append(Read());
    //                rtn = digit(cache.ToString());
    //            }
    //        }
    //        if (rtn == DigitReturns.IsMatch) break;
    //        s.Append(cache.ToString());
    //        cache.Clear();
    //    };

    //    var result = new CloseTokenSet()
    //    {
    //        Token = s.ToString(),
    //        CloseToken = cache.ToString()
    //    };
    //    return result;
    //}
}

public enum DigitReturns
{
    Unknown = 0,
    IsMatch = 1,
    IsNotMatch = 2
}

public class CloseTokenSet
{
    public string Token { get; set; } = string.Empty;
    public string CloseToken { get; set; } = string.Empty;
}
