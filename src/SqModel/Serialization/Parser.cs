using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class Parser : IDisposable
{
    private bool disposedValue;


    //空白が出てくるまで読む、
    //読んだ値がキーワードか見る
    //　-のあとに-が来れば行コメント(行末まで読み込み）
    //　/のあとに*が来れば複数行コメント（コメント終端まで読み込み）
    //　*のあとに/がくれば複数行コメントの終了
    //　(が来るとインラインSQLの可能性があるが・・・一旦無視
    //  --
    //  /*

    public Parser(string text)
    {
        Reader = new StringReader(text);
    }

    public StringReader Reader { get; init; }

    //public SelectQuery Parse()
    //{
    //    var q = new SelectQuery();

    //    // read to keyword 'select'
    //    var keyword = ReadToKeywordOrDefault(new() { "select" });
    //    if (keyword.ToLower() != "select") throw new Exception();

    //    //columns part


    //    while (fn()) { }

    //    //alias part


    //    return q;
    //}

    public void ParseColumn(SelectQuery q)
    {
        var col = new SelectColumn();
        var fn = () =>
        {
            var s = ReadUntil("., \r\n");
            var c = Peek();
            if (c == '.')
            {
                col.TableName = s;
                return true;
            }
            else if (c == ',')
            {
                col.ColumnName = s;
                Read();
                ParseColumn(q);
                return false;
            }
            else
            {
                col.ColumnName = s;
                ParseAlias(q, col);
                return false;
            }
        };

        while (fn()) { };

        if (col.TableName != String.Empty)
        {
            q.Select($"{col.TableName}.{col.ColumnName}", col.AliasName);
        }
        else
        {
            q.Select($"{col.ColumnName}", col.AliasName);
        }
    }

    public void ParseAlias(SelectQuery q, SelectColumn col)
    {
        ReadSkipSpaces();

        //, ...
        //from ...
        //alias_name, ...
        //alias_name from... 
        //as alias_name,
        //as alias_name from... 

        var sb = new StringBuilder();
        var fn = () =>
        {
            var s = ReadUntil(", af\r\n\t");
            sb.Append(s);
            var c = Peek();
            if (c == ',' || c == ' ')
            {
                if (sb.Length != 0) col.AliasName = sb.ToString();
                return false;
            }
            else if (s == string.Empty && c == 'a')
            {
                var key = ReadKeywordOrDefault(new() { "as" });
                if (key.ToLower() == "as" && Peek().IsSpace())
                {
                    ReadSkipSpaces();
                    col.AliasName = ReadUntilSpace();
                    return false;
                }
                sb.Append(key);
                sb.Append(ReadUntilSpace());
                return false;
            }
            else if (s.ToLower() == "from" && Peek().IsSpace())
            {
                ReadSkipSpaces();
                //TODO : from parse
            }
            sb.Append(s);
            return true;
        };
        fn();
        if (col.AliasName == String.Empty) col.AliasName = col.ColumnName;
    }


    public string GetKeywordOrDefault(List<string> keywords, string text)
    {
        if (!Peek().IsSpace()) return string.Empty;
        var lst = new List<string>();
        keywords.ForEach(keyword => lst.Add(keyword.ToLower()));

        return keywords.Where(x => x.ToLower() == text.ToLower()).FirstOrDefault() ?? string.Empty;
    }

    public string ReadKeywordOrDefault(List<string> keywords)
    {
        var sb = new StringBuilder();
        var lst = new List<string>();
        lst.AddRange(keywords);

        var fn = () =>
        {
            var c = Peek();
            lst = lst.Where(x => x.IsFirstChar(c)).Select(x => x.Substring(1, x.Length - 1)).ToList();
            if (!lst.Any()) return false;

            sb.Append(Read());
            if (Peek().IsSpace()) return false;

            return true;
        };

        while (fn()) { }
        return sb.ToString();
    }

    public char Read()
    {
        var i = Reader.Read();
        if (i.IsEof()) throw new EndOfStreamException();
        return (char)i;
    }

    public char Peek()
    {
        var i = Reader.Peek();
        if (i.IsEof()) throw new EndOfStreamException();
        return (char)i;
    }

    public string MultipleComment(StringBuilder sb)
    {
        if (sb.ToString() != "/") throw new Exception();
        if (Peek() != '*') throw new Exception();

        var fn = () =>
        {
            sb.Append(ReadUntil(x => "*".IndexOf(x) >= 0));
            if (Peek() == '/')
            {
                sb.Append(Read());
                return false;
            }
            sb.Append(Read());
            return true;
        };

        while (fn()) { }
        return sb.ToString();
    }

    //public bool ReadTo(char c)
    //{
    //    //この文字が出てくるまで読み込む
    //}


    public bool IsOneOf(string s)
    {
        var i = Reader.Peek();
        if (i.IsNotEof() || s.IndexOf(i.ToChar()) < 0) return false;
        Reader.Read();
        return true;
    }

    public char OneOf(string s)
    {
        var i = Reader.Peek();
        if (IsOneOf(s)) return i.ToChar();
        throw new Exception($"OneOf: '{(char)i}' is not in {s}");
    }

    public string ReadUntilSpace()
    {
        var digit = (char c) => !c.IsSpace();
        return ReadUntil(digit);
    }

    //public char? ReadChar(string chars)
    //{
    //    var digit = (char c) => chars.Select(x => c == x.ToLower() || c == x.ToUpper()).Any();
    //    return ReadChar(digit);
    //}

    public char? ReadChar(Func<char, bool> digit)
    {
        var i = Reader.Peek();
        if (i.IsNotEof() && digit(i.ToChar()))
        {
            Reader.Read();
            return (char)i;
        }
        return null;
    }

    public string ReadUntil(string chars)
    {
        var sb = new StringBuilder();
        chars.ToList().ForEach(x =>
        {
            var lower = x.ToLower();
            var upper = x.ToUpper();
            sb.Append(lower);
            if (lower != upper) sb.Append(upper);
        });
        var s = sb.ToString();
        var digit = (char c) =>
        {
            return s.IndexOf(c) == -1;
        };
        return ReadUntil(digit);
    }

    public string ReadUntil(Func<char, bool> digit)
    {
        var sb = new StringBuilder();
        var fn = () =>
        {
            var i = Reader.Peek();
            if (i.IsNotEof() && digit(i.ToChar()))
            {
                sb.Append(i.ToChar());
                Reader.Read();
                return true;
            }
            return false;
        };
        while (fn()) { }
        return sb.ToString();
    }

    public bool IsSpace(char c) => " \r\n\t".IndexOf(c) >= 0;

    public int ReadSkipSpaces()
    {
        return ReadSkipWhile(x => x.IsSpace());
    }

    public int ReadSkipWhile(Func<char, bool> digit)
    {
        var i = Reader.Peek();
        if (i.IsNotEof() && digit(i.ToChar()))
        {
            Reader.Read();
            return ReadSkipWhile(digit);
        }
        return i;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                Reader.Dispose();
            }

            // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
            // TODO: 大きなフィールドを null に設定します
            disposedValue = true;
        }
    }

    // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
    // ~Parser()
    // {
    //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
