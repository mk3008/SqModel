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

    public SelectTokenSet Parse()
    {
        ReadWhileSpace();

        var result = ReadUntilToken(new[] { "select" });
        if (result.NextToken != "select") throw new SyntaxException("select token is not found.");

        var tokenset = new SelectTokenSet();

        result = ParseSelectColumn(tokenset);

        //if (result.NextToken == "from");

        return tokenset;
    }
}

public class SyntaxException : Exception
{
    public SyntaxException(string? message) : base(message) { }
}