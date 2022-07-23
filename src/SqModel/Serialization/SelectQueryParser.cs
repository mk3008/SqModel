using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SelectQueryParser : Parser
{
    public SelectQueryParser(string text) : base(text)
    {
    }

    //public List<string> Parse()
    //{
    //    ReadWhileSpace();

    //    var r = ReadToken();
    //    var ts = new TokenSet() { Result = r };
    //    ts.Token = r.Token;

    //    if (r.Token == "select")
    //    {
    //        ts.Token = r.Token;
    //        ts.Splitter = ",";
    //        while (true)
    //        {
    //            var p = Parse();


    //            ts.InnerTokenSets.Add(p);
    //            if (p.Result.NextChar.IsSymbol()) continue;
    //            break;
    //        }
    //        return ts;
    //    }

    //    if (r.Token == "from")
    //    {
    //        while (true)
    //        {
    //            var p = Parse();
    //            if (p.Token == "where" || p.Token == ";" || p.Token == "") break;
    //            ts.InnerTokenSets.Add(p);
    //        }
    //        return ts;
    //    }

    //    if (r.Token == "where")
    //    {
    //        while (true)
    //        {
    //            var p = Parse();
    //            if (p.Token == ";" || p.Token == "") break;
    //            ts.InnerTokenSets.Add(p);
    //        }
    //        return ts;
    //    }

    //    if (r.Token == ".")
    //    {
    //        var ir = Parse();
    //        ts.InnerTokenSets.Add(ir.TokenSet);

    //        ir = Parse();
    //        if (ir.TokenSet.Token == "as")
    //        {
    //            ts.InnerTokenSets.Add(ir.TokenSet);
    //            return new ParseResult() { TokenSet = ts };
    //        }
    //        return new ParseResult() { TokenSet = ts, NextTokenSet = ir.TokenSet };
    //    }

    //    if (r.Token == "as") return new ParseResult() { TokenSet = ts };

    //    if (r.Token == "--")
    //    {
    //        ts.Token = string.Empty;
    //        ts.StartBracket = r.Token;
    //        ts.EndBracket = "\r\n";
    //        ts.InnerTokenSets.Add(new TokenSet() { Token = ReadUntilCrLf() });
    //        return new ParseResult() { TokenSet = ts };
    //    }

    //    if (r.Token == "/*")
    //    {
    //        ts.Token = string.Empty;
    //        ts.StartBracket = r.Token;
    //        ts.EndBracket = "*/";
    //        ts.InnerTokenSets.Add(new TokenSet() { Token = ReadUntilToken(new[] { ts.EndBracket }).Token });
    //        return new ParseResult() { TokenSet = ts };
    //    }

    //    if (r.Token == "(")
    //    {
    //        ts.Token = string.Empty;
    //        ts.StartBracket = r.Token;
    //        ts.EndBracket = ")";

    //        while (true)
    //        {
    //            var p = Parse();
    //            ts.InnerTokenSets.Add(p);
    //            if (p.Result.NextChar?.ToString() == ts.EndBracket)
    //            {
    //                Read();
    //                break;
    //            }
    //        }
    //        return ts;
    //    }
    //    return ts;
    //    //return new TokenSet() { Result = token, Token = token.Token };
    //}
}

public class SyntaxException : Exception
{
    public SyntaxException(string? message) : base(message) { }
}

public class ParseResult
{
    public TokenSet TokenSet { get; set; } = new();
    public TokenSet? NextTokenSet { get; set; }
}
