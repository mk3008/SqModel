//using SqModel.Analysis.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SqModel.Analysis;

//public partial class TokenReader
//{
//    public string CurrentToken { get; private set; } = string.Empty;

//    public void ClearCache() => CurrentToken = string.Empty;

//    public IEnumerable<string> ReadTokens(bool includeCurrentToken = false)
//    {
//        if (includeCurrentToken && !string.IsNullOrEmpty(CurrentToken))
//        {
//            yield return CurrentToken;
//        }

//        foreach (var item in ReadTokensCore())
//        {
//            CurrentToken = item;
//            yield return CurrentToken;
//        }
//        CurrentToken = string.Empty;
//    }

//    private IEnumerable<string> ReadTokensCore(bool skipSpace = true)
//    {
//        foreach (var word in ReadWords(skipSpace: skipSpace))
//        {
//            if (word.AreEqual("inner") || word.AreEqual("cross"))
//            {
//                var next = ReadWord();
//                if (!next.AreEqual("join")) throw new SyntaxException($"{word} {next}");
//                yield return $"{word} {next}";
//            }
//            else if (word.AreEqual("group") || word.AreEqual("partiton") || word.AreEqual("order"))
//            {
//                var next = ReadWord();
//                if (!next.AreEqual("by")) throw new SyntaxException($"{word} {next}");
//                yield return $"{word} {next}";
//            }
//            else if (word.AreEqual("left") || word.AreEqual("right"))
//            {
//                var c = PeekOrDefault();
//                if (c == null || c.Value == '(') yield return word;

//                var next = ReadWord();
//                if (next.AreEqual("outer")) next = ReadWord();
//                if (!next.AreEqual("join")) throw new SyntaxException($"{word} {next}");
//                yield return $"{word} {next}";
//            }
//            else if (word.AreEqual("is"))
//            {
//                var next = ReadWord();
//                if (string.IsNullOrEmpty(next))
//                {
//                    yield return word;
//                }
//                else if (!next.AreEqual("not"))
//                {
//                    yield return word;
//                    yield return next;
//                }
//                else
//                {
//                    yield return $"{word} {next}";
//                }
//            }
//            else if (word.AreEqual("case"))
//            {
//                var next = ReadWord();
//                if (string.IsNullOrEmpty(next))
//                {
//                    yield return word;
//                }
//                else if (!next.AreEqual("when"))
//                {
//                    yield return word;
//                    yield return next;
//                }
//                else
//                {
//                    yield return $"{word} {next}";
//                }
//            }
//            else if (word.AreEqual("("))
//            {
//                yield return word;

//                //inner text
//                var level = 1;
//                var token = word;
//                var inner = new StringBuilder();

//                while (level == 0 || string.IsNullOrEmpty(token))
//                {
//                    token = ReadTokensCore(skipSpace: false).FirstOrDefault();
//                    if (token == null) break;
//                    if (token.AreEqual(")"))
//                    {
//                        level--;
//                        if (level == 0)
//                        {
//                            yield return inner.ToString();
//                            yield return token;
//                        }
//                    }
//                    else
//                    {
//                        inner.Append(token);
//                        if (token.AreEqual("(")) level++;
//                    }
//                }
//                if (level != 0) throw new SyntaxException("bracket is not closed");
//            }
//            else if (word.AreEqual("--"))
//            {
//                //skip line comment
//                SkipUntilLineEnd();
//            }
//            else if (word.AreEqual("/*"))
//            {
//                //skip block comment
//                var level = 1;
//                var token = word;
//                while (level == 0 || string.IsNullOrEmpty(token))
//                {
//                    token = ReadWords().Where(x => x.AreEqual("/*") || x.AreEqual("*/")).FirstOrDefault(); ;
//                    if (token == null) break;

//                    if (token.AreEqual("/*"))
//                    {
//                        level++;
//                    }
//                    else if (token.AreEqual("*/"))
//                    {
//                        level--;
//                    }
//                }
//                if (level != 0) throw new SyntaxException("block comment is not closed");
//            }
//            else if (word.AreEqual(";"))
//            {
//                break;
//            }
//            yield return word;
//        }
//    }
//}
