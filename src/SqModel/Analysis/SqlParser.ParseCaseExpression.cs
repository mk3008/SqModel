using SqModel.Analysis;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public IValueClause ParseCaseExpression()
    {
        var q = ReadTokensWithoutComment();

        var token = CurrentToken.IsNotEmpty() ? CurrentToken : q.First();
        var next = q.First();

        if (token.ToLower() == "case" && next.ToLower() == "when")
        {
            return new CaseWhenExpressionParser(this).Parse();
        }
        if (token.ToLower() == "case")
        {
            return new CaseExpressionParser(this).Parse();
        }
        throw new SyntaxException("case expression parse error.");
    }
}