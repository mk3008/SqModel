using SqModel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public ICommand ParseCaseExpression(bool includeCurrentToken = false)
    {
        var token = (includeCurrentToken) ? CurrentToken : ReadToken();
        var next = ReadToken();
        if (token.ToLower() == "case" && next.ToLower() == "when") return new CaseWhenExpressionParser(this).Parse();
        if (token.ToLower() == "case") return new CaseExpressionParser(this).Parse();
        throw new SyntaxException("case expression parse error.");
    }
}
