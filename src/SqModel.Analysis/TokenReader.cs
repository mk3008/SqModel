using Cysharp.Text;
using SqModel.Analysis.Extensions;
using SqModel.Core;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SqModel.Analysis;

public class TokenReader : LexReader
{
    public TokenReader(string text) : base(text)
    {
    }

    //public Action<string>? Logger { get; set; }

    private string? TokenCache { get; set; } = string.Empty;

    //public IEnumerable<string> ReadTokens()
    //{
    //    foreach (var item in ReadTokensCore())
    //    {
    //        yield return item;
    //    }
    //}

    public string? PeekToken()
    {
        if (string.IsNullOrEmpty(TokenCache))
        {
            TokenCache = ReadTokensCore(skipSpace: true).FirstOrDefault();
        }
        return TokenCache;
    }

    public string? TryReadToken(string expectToken)
    {
        var s = PeekToken();
        if (!s.AreEqual(expectToken)) return null;
        return ReadToken();
    }

    public string ReadToken(string expectToken)
    {
        var s = PeekToken();
        if (string.IsNullOrEmpty(s)) throw new SyntaxException($"expect '{expectToken}', actual is empty.");
        if (!s.AreEqual(expectToken)) throw new SyntaxException($"expect '{expectToken}', actual '{s}'.");
        return ReadToken();
    }

    public string ReadToken(bool skipComment = true)
    {
        string? token = ReadTokenCore();
        if (token == null) return string.Empty;
        if (!skipComment) return token;

        if (token == "--" || token == "/*")
        {
            ReadTokenCore();
            return ReadToken(skipComment);
        }
        return token;
    }

    public string? ReadTokenCore()
    {
        if (!string.IsNullOrEmpty(TokenCache))
        {
            var s = TokenCache;
            TokenCache = string.Empty;
            return s;
        }
        return ReadTokensCore(skipSpace: true).FirstOrDefault();
    }

    private IEnumerable<string> ReadTokensCore(bool skipSpace = true)
    {
        foreach (var word in ReadLexs(skipSpace: skipSpace))
        {
            if (word.AreEqual(")"))
            {
                yield return word;
                continue;
            }

            if (word.AreEqual("inner") || word.AreEqual("cross"))
            {
                var next = ReadLex();
                if (!next.AreEqual("join")) throw new SyntaxException($"near {word}");
                yield return word + " " + next;
                continue;
            }

            if (word.AreEqual("group") || word.AreEqual("partition") || word.AreEqual("order"))
            {
                var next = ReadLex();
                if (!next.AreEqual("by")) throw new SyntaxException($"near {word}");
                yield return word + " " + next;
                continue;
            }

            if (word.AreEqual("left") || word.AreEqual("right"))
            {
                var c = PeekOrDefault();
                if (c == null || c.Value == '(')
                {
                    yield return word;
                    continue;
                }

                var next = ReadLex();
                var sb = ZString.CreateStringBuilder();
                sb.Append(word);
                if (next.AreEqual("outer"))
                {
                    sb.Append(" " + next);
                    next = ReadLex();
                }
                if (!next.AreEqual("join")) throw new SyntaxException($"near {word}");
                sb.Append(" " + next);
                yield return sb.ToString();
                continue;
            }

            if (word.AreEqual("is"))
            {
                var next = ReadLex();
                if (string.IsNullOrEmpty(next))
                {
                    throw new SyntaxException($"near {word}");
                }

                if (!next.AreEqual("not"))
                {
                    yield return word;
                    yield return next;
                }
                else
                {
                    yield return word + " " + next;
                }
                continue;
            }

            if (word.AreEqual("case"))
            {
                yield return word; //case 
                SkipSpace();
                var cnd = ReadUntilToken("when");
                if (string.IsNullOrEmpty(cnd)) yield return cnd; //t.c 

                var exp = ReadUntilToken("end");
                yield return "when" + exp; //when t.c = 1 else 2
                yield return "end"; //end

                continue;
            }

            if (word.AreEqual("("))
            {
                yield return word;
                var (_, inner) = ReadUntilCloseBracket();
                yield return inner;
                yield return ")";
                continue;
            }

            if (word.AreEqual("--"))
            {
                yield return word;
                yield return ReadUntilLineEnd();
                continue;
            }

            if (word.AreEqual("/*"))
            {
                yield return word;
                yield return ReadUntilCloseBlockComment();
                continue;
            }

            if (word.AreEqual("nulls"))
            {
                var next = ReadLex();
                if (next.AreEqual("first") || next.AreEqual("last"))
                {
                    yield return word + " " + next;
                }
                else
                {
                    yield return word;
                    yield return next;
                }
                continue;
            }

            if (word.AreEqual(";"))
            {
                break;
            }
            yield return word;
        }
    }

    internal (string first, string inner) ReadUntilCloseBracket()
    {
        var sb = ZString.CreateStringBuilder();
        var fs = string.Empty;

        foreach (var word in ReadTokensCore(skipSpace: false))
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

        foreach (var word in ReadTokensCore(skipSpace: false))
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

        foreach (var word in ReadTokensCore(skipSpace: false))
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
        foreach (var word in ReadTokensCore(skipSpace: false))
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