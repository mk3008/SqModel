using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public IText ParseParenthese()
    {
        ReadSkipSpaces();

        var cn = PeekOrDefault();
        if (cn == null) throw new InvalidOperationException();
        var c = cn.Value;

        if (c != '(') throw new NotSupportedException();
        Read();
        var p = new ParenthesesText() { OpenSymbol = "(", CloseSymbol = ")" };

        var fn = () => {
            var text = ReadUntil("()");
            if (text != string.Empty) p.InnerTexts.Add(new PlainText() { Value = text });
            if (Peek() == ')')
            {
                Read();
                return false;
            }
            p.InnerTexts.Add(ParseParenthese());
            return true;
        };

        while (fn()) { };
        return p;
    }
}
