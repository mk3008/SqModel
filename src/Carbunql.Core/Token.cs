using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Carbunql.Core;

public class Token
{
    public static Token BracketStart(object sender, Token? parent)
    {
        return new Token(sender, parent, "(", false);
    }

    public static Token BracketEnd(object sender, Token? parent)
    {
        return new Token(sender, parent, ")", false);
    }

    public static Token Dot(object sender, Token? parent)
    {
        return new Token(sender, parent, ".", false);
    }

    public static Token Comma(object sender, Token? parent)
    {
        return new Token(sender, parent, ",", false);
    }

    public static Token Reserved(object sender, Token? parent, string text)
    {
        return new Token(sender, parent, text, true);
    }

    public Token(object sender, Token? parent, string text, bool isReserved = false)
    {
        Sender = sender;
        Parent = parent;
        Text = text;
        IsReserved = isReserved;
    }

    public object Sender { get; init; }

    public Token? Parent { get; init; }

    public string Text { get; init; }

    public bool IsReserved { get; init; }

    public IEnumerable<Token> Parents()
    {
        if (Parent == null) yield break;
        yield return Parent;
        foreach (var item in Parent.Parents()) yield return item;
    }
}