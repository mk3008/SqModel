using SqModel.Extension;
using System.Text;

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

    public string ReadWhileSpace()
    {
        var digit = (char c) => c.IsSpace();
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