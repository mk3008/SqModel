using Cysharp.Text;
using SqModel.Analysis.Extensions;
using SqModel.Core;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SqModel.Analysis;

public class TokenReader : LexReader
{
    public TokenReader(string text) : base(text)
    {
    }

    private string? TokenCache { get; set; } = string.Empty;

    public string? PeekRawToken()
    {
        if (string.IsNullOrEmpty(TokenCache))
        {
            TokenCache = ReadRawToken(skipSpace: true);
        }
        return TokenCache;
    }

    public string? TryReadToken(string expectRawToken)
    {
        var s = PeekRawToken();
        if (!s.AreEqual(expectRawToken)) return null;
        return ReadToken();
    }

    public string ReadToken(string expectRawToken)
    {
        var s = PeekRawToken();
        if (string.IsNullOrEmpty(s)) throw new SyntaxException($"expect '{expectRawToken}', actual is empty.");
        if (!s.AreEqual(expectRawToken)) throw new SyntaxException($"expect '{expectRawToken}', actual '{s}'.");
        return ReadToken();
    }

    public string ReadToken(string[] expectRawTokens)
    {
        var s = PeekRawToken();
        if (string.IsNullOrEmpty(s)) throw new SyntaxException($"token is empty.");
        if (!s.AreContains(expectRawTokens)) throw new SyntaxException($"near '{s}'.");
        return ReadToken();
    }

    public string ReadToken(bool skipComment = true)
    {
        string? token = ReadRawToken();
        if (string.IsNullOrEmpty(token)) return string.Empty;

        // Explore possible two-word tokens
        if (token.AreEqual("is"))
        {
            if (PeekRawToken().AreEqual("not"))
            {
                return token + " " + ReadToken("not");
            }
            return token;
        }

        if (token.AreContains(new string[] { "inner", "cross" }))
        {
            var outer = TryReadToken("outer");
            if (!string.IsNullOrEmpty(outer)) token += " " + outer;
            var t = ReadToken("join");
            return token + " " + t;
        }

        if (token.AreContains(new string[] { "group", "partition", "order" }))
        {
            var t = ReadToken("by");
            return token + " " + t;
        }

        if (token.AreContains(new string[] { "left", "right" }))
        {
            if (PeekRawToken().AreEqual("(")) return token;
            var outer = TryReadToken("outer");
            if (!string.IsNullOrEmpty(outer)) token += " " + outer;
            var t = ReadToken("join");
            return token + " " + t;
        }

        if (token.AreEqual("nulls"))
        {
            var t = ReadToken(new string[] { "first", "last" });
            return token + " " + t;
        }

        if (token.AreEqual("union"))
        {
            if (PeekRawToken().AreEqual("all"))
            {
                return token + " " + ReadToken("all");
            }
            return token;
        }

        if (!skipComment) return token;

        if (token == "--")
        {
            ReadUntilLineEnd();
            return ReadToken(skipComment);
        }

        if (token == "/*")
        {
            ReadUntilCloseBlockComment();
            return ReadToken(skipComment);
        }
        return token;
    }

    public string? ReadRawToken(bool skipSpace = true)
    {
        if (!string.IsNullOrEmpty(TokenCache))
        {
            var s = TokenCache;
            TokenCache = string.Empty;
            return s;
        }
        return ReadLexs(skipSpace).FirstOrDefault();
    }

    private IEnumerable<string> ReadRawTokens(bool skipSpace = true)
    {
        var token = ReadRawToken(skipSpace: skipSpace);
        while (!string.IsNullOrEmpty(token))
        {
            yield return token;
            token = ReadRawToken(skipSpace: skipSpace);
        }
    }

    internal (string first, string inner) ReadUntilCloseBracket()
    {
        var sb = ZString.CreateStringBuilder();
        var fs = string.Empty;

        foreach (var word in ReadRawTokens(skipSpace: false))
        {
            if (word == null) break;
            if (string.IsNullOrEmpty(fs)) fs = word;

            if (word.AreEqual(")"))
            {
                return (fs, sb.ToString());
            }

            if (word.AreEqual("("))
            {
                var (_, inner) = ReadUntilCloseBracket();
                sb.Append("(" + inner + ")");
            }
            else
            {
                sb.Append(word);
            }
        }

        throw new SyntaxException("bracket is not closed");
    }

    private string ReadUntilCloseBlockComment()
    {
        var inner = ZString.CreateStringBuilder();

        foreach (var word in ReadRawTokens(skipSpace: false))
        {
            if (word == null) break;

            inner.Append(word);
            if (word.AreEqual("*/"))
            {
                return inner.ToString();
            }
            if (word.AreEqual("/*"))
            {
                inner.Append(ReadUntilCloseBlockComment());
            }
        }

        throw new SyntaxException("block comment is not closed");
    }

    internal string ReadUntilCaseExpressionEnd()
    {
        var inner = ZString.CreateStringBuilder();

        foreach (var word in ReadRawTokens(skipSpace: false))
        {
            if (word == null) break;

            inner.Append(word);
            if (word.TrimStart().AreEqual("end"))
            {
                return inner.ToString();
            }
            if (word.TrimStart().AreEqual("case"))
            {
                inner.Append(ReadUntilCaseExpressionEnd());
            }
        }

        throw new SyntaxException("case expression is not end");
    }

    internal string ReadUntilToken(string breaktoken)
    {
        return ReadUntilToken(x => x.AreEqual(breaktoken));
    }

    internal string ReadUntilToken(Func<string, bool> fn)
    {
        var inner = ZString.CreateStringBuilder();

        SkipSpace();
        foreach (var word in ReadRawTokens(skipSpace: false))
        {
            if (word == null) break;
            if (fn(word.TrimStart()))
            {
                return inner.ToString();
            }
            inner.Append(word);
        }

        throw new SyntaxException($"breaktoken token is not found");
    }
}