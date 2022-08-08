using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

//public interface ITokenSet
//{
//    IEnumerable<string> GetTokens();

//    public ReadTokenResult Result { get; }
//}

public class TokenSet //: ITokenSet
{
    //public TokenSet()
    //{
    //    Result = new();
    //}
    //public TokenSet(ReadTokenResult result)
    //{
    //    Result = result;
    //}
    public string ConjunctionToken { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string StartBracket { get; set; } = string.Empty;

    public List<TokenSet> InnerTokenSets { get; set; } = new();

    public string Splitter { get; set; } = string.Empty;

    public string EndBracket { get; set; } = string.Empty;

    public IEnumerable<string> GetTokens()
    {
        if (!string.IsNullOrEmpty(ConjunctionToken)) yield return ConjunctionToken;
        if (!string.IsNullOrEmpty(Token)) yield return Token;
        if (!string.IsNullOrEmpty(StartBracket)) yield return StartBracket;
        foreach(var x in InnerTokenSets) foreach(var y in x.GetTokens()) yield return y;
        if (!string.IsNullOrEmpty(EndBracket)) yield return EndBracket;
    }

    //public ReadTokenResult Result { get; set; } = new();
}

//public class ValueToken : ITokenSet
//{
//    public ValueToken()
//    {
//        Result = new();
//    }
//    public ValueToken(ReadTokenResult result)
//    {
//        Result = result;
//    }

//    public string Conjunction { get; set; } = string.Empty;

//    public string Owner { get; set; } = string.Empty;

//    public string Value { get; set; } = string.Empty;

//    public string Alias { get; set; } = string.Empty;

//    public IEnumerable<string> GetTokens()
//    {
//        if (string.IsNullOrEmpty(Conjunction)) yield return Conjunction;
//        if (string.IsNullOrEmpty(Owner))
//        {
//            yield return Owner;
//            yield return ".";
//        }
//        if (string.IsNullOrEmpty(Value)) yield return Value;
//        if (string.IsNullOrEmpty(Alias))
//        {
//            yield return "as";
//            yield return Alias;
//        }
//    }

//    public ReadTokenResult Result { get; init; } 
//}

