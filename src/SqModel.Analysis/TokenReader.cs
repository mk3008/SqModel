﻿using Cysharp.Text;
using SqModel.Analysis.Extensions;
using SqModel.Core;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace SqModel.Analysis;

public class TokenReader : WordReader
{
    public TokenReader(string text) : base(text)
    {
    }

    public Action<string>? Logger { get; set; }

    private string? TokenCache { get; set; } = string.Empty;

    public IEnumerable<string> ReadTokens()
    {
        foreach (var item in ReadTokensCore())
        {
            yield return item;
        }
    }

    public string? PeekToken()
    {
        if (string.IsNullOrEmpty(TokenCache))
        {
            TokenCache = ReadTokensCore(skipSpace: true).FirstOrDefault();
        }
        return TokenCache;
    }

    //public bool PeekTokenAreEqual(string expect)
    //{
    //    return PeekToken().AreEqual(expect);
    //}

    //public bool PeekTokenAreContain(IEnumerable<string> expects)
    //{
    //    return PeekToken().AreContains(expects);
    //}

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
        foreach (var word in ReadWords(skipSpace: skipSpace))
        {
            if (word.AreEqual(")"))
            {
                yield return word;
                continue;
            }

            if (word.AreEqual("inner") || word.AreEqual("cross"))
            {
                var next = ReadWord();
                if (!next.AreEqual("join")) throw new SyntaxException($"near {word}");
                yield return word + " " + next;
                continue;
            }

            if (word.AreEqual("group") || word.AreEqual("partition") || word.AreEqual("order"))
            {
                var next = ReadWord();
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

                var next = ReadWord();
                var sb = ZString.CreateStringBuilder();
                sb.Append(word);
                if (next.AreEqual("outer"))
                {
                    sb.Append(" " + next);
                    next = ReadWord();
                }
                if (!next.AreEqual("join")) throw new SyntaxException($"near {word}");
                sb.Append(" " + next);
                yield return sb.ToString();
                continue;
            }

            if (word.AreEqual("is"))
            {
                var next = ReadWord();
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
                SkipSpace();
                var cnd = ReadUntilToken("when");
                var exp = ReadUntilToken("end");
                if (string.IsNullOrEmpty(cnd))
                {
                    //case when t.c = 1 else 2 end
                    yield return word; //case 
                    yield return "when" + exp; //when t.c = 1 else 2
                    yield return "end"; //end
                }
                else
                {
                    //case t.c when 1 else 2 end
                    yield return word; //case 
                    yield return cnd; //t.c 
                    yield return "when" + exp; //when 1 else 2
                    yield return "end"; //end
                }
                continue;
            }

            if (word.AreEqual("("))
            {
                yield return word;
                var (inner, closer) = ReadUntilCloseBracket();
                yield return inner;
                yield return closer;
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

            if (word.AreEqual(";"))
            {
                break;
            }
            yield return word;
        }
    }

    internal (string inner, string closer) ReadUntilCloseBracket()
    {
        var inner = ZString.CreateStringBuilder();

        foreach (var word in ReadTokensCore(skipSpace: false))
        {
            if (word == null) break;
            if (word.AreEqual(")"))
            {
                return (inner.ToString(), word);
            }
            inner.Append(word);
            if (word.AreEqual("("))
            {
                var (x, y) = ReadUntilCloseBracket();
                inner.Append(x);
                inner.Append(y);
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

    private string ReadUntilToken(string breaktoken)
    {
        var inner = ZString.CreateStringBuilder();

        foreach (var word in ReadTokensCore(skipSpace: false))
        {
            if (word == null) break;
            if (word.TrimStart().AreEqual(breaktoken))
            {
                return inner.ToString();
            }
            inner.Append(word);
        }

        throw new SyntaxException($"{breaktoken} token is not found");
    }
}