using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public static class SingleQuoteParser
{
    private static string COMMAND = "'";

    private static string PATTERN = $@"{COMMAND}";

    public static ICommandText Parse(string text)
    {

        var texts = new CommandTextCollection();

        var index = 0;

        var ms = Regex.Matches(text, PATTERN);
        var isCatched = false;
        var isSkip = false;

        foreach (Match m in ms)
        {
            if (m.Success && m.Value == COMMAND && !isCatched)
            {
                if (m.Index != index)
                {
                    //plan text
                    var value = text.Substring(index, m.Index - index);
                    texts.Add(value);
                }

                isCatched = true;
                index = m.Index + 1; //skip start command char
                continue;
            }

            if (m.Success && m.Value == COMMAND && isCatched && isSkip)
            {
                isSkip = false;
                continue;
            }

            if (m.Success && m.Value == COMMAND && isCatched && !isSkip)
            {
                if (m.Index != text.Length - 1 && text.Substring(m.Index + 1, 1) == COMMAND)
                {
                    isSkip = true;
                    continue;
                }

                var g = new SingleQuoteCommandText() { Value = text.Substring(index, m.Index - index) };
                texts.Add(g);

                isCatched = false;
                index = m.Index + 1; //skip end command char
            }
        }

        if (index != text.Length ) texts.Add(text.Substring(index, text.Length - index));

        return texts;
    }
}
