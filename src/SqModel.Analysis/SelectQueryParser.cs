using SqModel.Analysis.Extensions;

namespace SqModel.Analysis;

public class SelectQueryParser : TokenReader
{
    public SelectQueryParser(string text) : base(text)
    {
    }

    public void Parse()
    {
        var token = ReadToken();
        //if (!(token.AreEqual("select") || token.AreEqual("with"))) throw new SyntaxException($"near {token} (expect: select or with)");

        if (token.AreEqual("select"))
        {

        }
    }
}