using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;


public static class ParrentheseParser
{
    private static char START_COMMAND = '(';

    private static char END_COMMAND = ')';

    private static string PATTERN = $@"(\{START_COMMAND}|\{END_COMMAND})";

    public static IText Parse(string text)
    {

        var texts = new TextCollection();

        var index = 0;
        var node = 0;

        var ms = Regex.Matches(text, PATTERN);
        var isCatched = false;

        foreach (Match m in ms)
        {
            if (m.Success && m.Value == START_COMMAND.ToString() && !isCatched)
            {
                if (m.Index != index)
                {
                    //plan text
                    var value = text.Substring(index, m.Index - index);
                    texts.Add(value, index);
                }

                isCatched = true;
                index = m.Index + 1; //skip start command char
                node++;
                continue;
            }

            if (m.Success && m.Value == START_COMMAND.ToString() && isCatched)
            {
                node++;
                continue;
            };

            if (m.Success && m.Value == END_COMMAND.ToString() && !isCatched) continue;



            if (m.Success && m.Value == END_COMMAND.ToString() && isCatched)
            {
                node--;
                if (node != 0) continue;

                var s = text.Substring(index, m.Index - index);
                var g = new GroupText() { Prefix = "(", Suffix = ")", Index = m.Index };
                g.Value = Parse(s);
                texts.Add(g);

                isCatched = false;
                index = m.Index + 1; //skip end command char
            }
        }

        if (index != text.Length) texts.Add(text.Substring(index, text.Length - index), index);

        return texts;
    }
}