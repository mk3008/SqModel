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
        //Level = 0;
        //RefreshIndent();
        //PrevToken = null;
        //TokenIndents.Add((Token, Level));
        //OnStartBlock(Token);
        return;
    }

    public int Level { get; set; }

    public string Indent { get; set; } = string.Empty;

    private Token? PrevToken { get; set; }

    public List<(Token Token, int Level)> TokenIndents { get; set; } = new();

    //public abstract string OnEndItemBeforeWriteToken(Token token);

    //public abstract string OnStartItemBeforeWriteToken(Token token);



    public virtual string WriteToken(Token token)
    {
        var isAppendSplitter = () =>
        {
            if (PrevToken == null) return false;

            if (PrevToken!.Text == "(") return false;
            if (PrevToken!.Text == ".") return false;
            //if (PrevToken!.Text == ")" && token.IsReserved) return false;

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
            PrevToken = null;
            return "\r\n" + Indent;
        }

        if (token.Text.AreEqual("else") || token.Text.AreEqual("when"))
        {
            PrevToken = null;
            return "\r\n" + Indent;
        }

        return string.Empty;
    }

    public virtual string OnAfterWriteToken(Token token)
    {
        if (token.Text == "," && token.Sender is SelectClause)
        {
            PrevToken = null;
            return "\r\n" + Indent;
        }

        if (token.Text == "," && token.Parent != null && token.Parent.Text.AreEqual("values"))
        {
            PrevToken = null;
            return "\r\n" + Indent;
        }

        return string.Empty;
    }

    public virtual string OnStartBlock(Token token)
    {
        //if (PrevToken != null && PrevToken.Sender is OperatableQuery) return string.Empty;
        //if (token.Sender is CommonTable) return string.Empty;
        //if (PrevToken?.Text == "(" && PrevToken.Parent?.Sender is SelectClause) return string.Empty;

        //Level++;

        //if (token.Parent == null) throw new Exception();

        //if (PrevToken != null)
        //{
        //    Logger?.Invoke("Level:" + Level + ", sender:" + token.Sender.GetType().Name + ", parent:" + token.Parent.Text + ", prev:" + PrevToken.Text + ", text:" + token.Text);
        //}
        //else
        //{
        //    Logger?.Invoke("Level:" + Level + ", sender:" + token.Sender.GetType().Name + ", parent:" + token.Parent.Text + ", " + token.Text);
        //}
        //TokenIndents.Add((token.Parent, Level));


        //root is not regist
        if (token.Parent == null) throw new Exception();

        if (token.Parent.Parent != null && token.Parent.Parent.Text.AreEqual("values")) return string.Empty;

        if (token.Parent.Sender is FunctionValue) return string.Empty;
        if (token.Parent.Sender is WindowFunction) return string.Empty;

        Level++;
        Logger?.Invoke(@$"OnStartBlock Indent:{Level}, parent:{token.Parent.Text}");
        TokenIndents.Add((token.Parent, Level));

        RefreshIndent();
        PrevToken = null;
        return "\r\n" + Indent;
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
        //if (token.Text == ")" && token.Parent?.Sender is SelectClause) return string.Empty;

        //if (token.Parent == null)
        //{
        //    if (token.Text == "," && token.Sender is WithClause) return string.Empty;
        //    Level = 0;
        //}
        //else
        //{
        //    if (!TokenIndents.Where(x => x.Token.Equals(token.Parent)).Any())
        //    {
        //        return string.Empty;
        //    }
        //    else
        //    {
        //        Level = TokenIndents.Where(x => x.Token.Equals(token.Parent)).Select(x => x.Level).FirstOrDefault();
        //    }
        //}


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
        else
        {
            var q = TokenIndents.Where(x => x.Token != null && x.Token.Equals(token.Parent)).Select(x => x.Level);
            if (!q.Any()) return string.Empty;
            Level = q.First();
            Logger?.Invoke(@$"OnEndBlock   Indent:{Level}, parent:{token.Parent.Text}");
        }

        RefreshIndent();
        PrevToken = null;
        return "\r\n" + Indent;
    }

    public virtual void OnEnd()
    {
        return;
    }

    private void RefreshIndent()
    {
        Indent = (Level * 4).ToSpaceString();
    }
}