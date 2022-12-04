using SqModel.Analysis.Builder;
using SqModel.Analysis.Extensions;
using SqModel.Core;
using SqModel.Core.Clauses;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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