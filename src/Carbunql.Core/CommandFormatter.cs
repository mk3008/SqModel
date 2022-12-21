using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;
using Carbunql.Core.Tables;
using Carbunql.Core.Values;

namespace Carbunql.Core;


public class CommandFormatter
{
    public virtual bool IsLineBreakOnBeforeWriteToken(Token token)
    {
        if (!token.Text.AreEqual("on") && token.Sender is Relation) return true;
        if (token.Text.AreEqual("else") || token.Text.AreEqual("when")) return true;
        if (token.Text.AreEqual("and"))
        {
            if (token.Sender is BetweenExpression) return false;
            if (token.Parent != null && token.Parent.Sender is WhereClause) return true;
            return false;
        }

        return false;
    }

    public virtual bool IsLineBreakOnAfterWriteToken(Token token)
    {
        if (token.Sender is OperatableQuery) return true;

        if (token.Text == "," && token.Sender is WithClause) return true;
        if (token.Text == "," && token.Sender is SelectClause) return true;
        if (token.Text == "," && token.Sender is GroupClause) return true;
        if (token.Text == "," && token.Sender is OrderClause) return true;
        if (token.Text == "," && token.Parent != null && token.Parent.Text.AreEqual("values")) return true;

        return false;
    }

    public virtual bool IsIncrementIndentOnBeforeWriteToken(Token token)
    {
        if (token.Sender is OperatableQuery) return false;

        if (token.Parent != null && token.Parent.Text.AreEqual("values")) return false;
        if (token.Sender is FunctionValue || token.Sender is WindowFunction) return false;
        if (token.Text == "(" && token.IsReserved == false) return false;

        return true;
    }

    public virtual bool IsDecrementIndentOnBeforeWriteToken(Token token)
    {
        if (token.Text == "," && token.Sender is WithClause) return false;

        if (token.Parent == null) return true;

        if (token.Parent != null && token.Parent.Text.AreEqual("values")) return false;
        if (token.Sender is FunctionValue || token.Sender is WindowFunction) return false;
        if (token.Text == ")" && token.IsReserved == false) return false;

        return true;
    }
}