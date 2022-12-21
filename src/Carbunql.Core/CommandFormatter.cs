using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;
using Carbunql.Core.Tables;
using Carbunql.Core.Values;

namespace Carbunql.Core;


public class CommandFormatter
{
    public Action<string>? Logger { get; set; }

    public virtual void OnStart(Token Token)
    {
        return;
    }

    public int Level { get; set; }

    public string Indent { get; set; } = string.Empty;

    private Token? PrevToken { get; set; }

    public List<(Token Token, int Level)> TokenIndents { get; set; } = new();

    public virtual string WriteToken(Token token)
    {
        var isAppendSplitter = () =>
        {
            if (PrevToken == null) return false;

            if (PrevToken!.Text == "(") return false;
            if (PrevToken!.Text == ".") return false;

            if (token.Text.StartsWith("::")) return false;
            if (token.Text == ")") return false;
            if (token.Text == ",") return false;
            if (token.Text == ".") return false;
            if (token.Text == "(")
            {
                if (token.Sender is VirtualTable) return true;
                if (token.Sender is FunctionValue) return false;
                return true;
            }

            return true;
        };

        var s = isAppendSplitter() ? " " : "";
        PrevToken = token;

        if (token.IsReserved) return s + token.Text.ToUpper();
        return s + token.Text;
    }

    public virtual string OnBeforeWriteToken(Token token)
    {
        if (PrevToken == null) return string.Empty;

        if (!token.Text.AreEqual("on") && !token.Sender.Equals(PrevToken.Sender) && token.Sender is Relation)
        {
            return GetLineBreakText();
        }

        if (token.Text.AreEqual("else") || token.Text.AreEqual("when"))
        {
            return GetLineBreakText();
        }

        return string.Empty;
    }

    public virtual string OnAfterWriteToken(Token token)
    {
        if (token.Text == "," && token.Sender is SelectClause)
        {
            return GetLineBreakText();
        }

        if (token.Text == "," && token.Parent != null && token.Parent.Text.AreEqual("values"))
        {
            return GetLineBreakText();
        }

        return string.Empty;
    }

    public virtual string OnStartBlock(Token token)
    {
        //root is not regist
        if (token.Parent == null) throw new Exception();

        //no line breaks
        if (token.Parent.Parent != null && token.Parent.Parent.Text.AreEqual("values")) return string.Empty;
        if (token.Parent.Sender is FunctionValue) return string.Empty;
        if (token.Parent.Sender is WindowFunction) return string.Empty;
        if (token.Parent.Text == "(" && token.Parent.IsReserved == false) return string.Empty;

        //line breaks but no indentation
        if (token.Parent.Sender is OperatableQuery)
        {
            Level--;
        }

        Level++;
        Logger?.Invoke(@$"OnStartBlock Indent:{Level}, parent:{token.Parent.Text}");
        TokenIndents.Add((token.Parent, Level));

        return GetLineBreakText();
    }

    public virtual string OnEndBlockAfterWriteToken(Token token)
    {
        if (token.Text == "," && token.Sender is WithClause)
        {
            Level--;
            RefreshIndent();
        }

        return string.Empty;
    }

    public virtual string OnEndBlockBeforeWriteToken(Token token)
    {
        if (token.Text == "," && token.Sender is WithClause)
        {
            return string.Empty;
        }

        if (token.Parent == null)
        {
            Level = 0;
            Logger?.Invoke(@$"OnEndBlock   Indent:{Level}, parent:NULL");
        }
        else if (token.Parent.Text.AreEqual("values"))
        {
            return string.Empty;
        }
        else if (token.Sender is FunctionValue || token.Sender is WindowFunction)
        {
            return string.Empty;
        }
        else if (token.Text == ")" && token.IsReserved == false)
        {
            return string.Empty;
        }
        else
        {
            var q = TokenIndents.Where(x => x.Token != null && x.Token.Equals(token.Parent)).Select(x => x.Level);
            if (!q.Any()) return string.Empty;
            Level = q.First();
            Logger?.Invoke(@$"OnEndBlock   Indent:{Level}, parent:{token.Parent.Text} text:{token.Text}");
        }

        return GetLineBreakText();
    }

    public virtual void OnEnd()
    {
        return;
    }

    private void RefreshIndent()
    {
        Indent = (Level * 4).ToSpaceString();
    }

    private string GetLineBreakText()
    {
        RefreshIndent();
        PrevToken = null;
        return "\r\n" + Indent;
    }
}