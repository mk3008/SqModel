using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;


public static class ParentheseParser
{
    private static string START_COMMAND = "(";

    private static string END_COMMAND = ")";

    private static string PATTERN = $@"(\{START_COMMAND}|\{END_COMMAND})";

    public static ICommandText Parse(string text)
    {

        var texts = new CommandTextCollection();

        var index = 0;
        var node = 0;

        var ms = Regex.Matches(text, PATTERN);
        var isCatched = false;

        foreach (Match m in ms)
        {
            if (m.Success && m.Value == START_COMMAND && !isCatched)
            {
                if (m.Index != index)
                {
                    //plan text
                    var value = text.Substring(index, m.Index - index);
                    texts.Add(value);
                }

                isCatched = true;
                index = m.Index + 1; //skip start command char
                node++;
                continue;
            }

            if (m.Success && m.Value == START_COMMAND && isCatched)
            {
                node++;
                continue;
            };

            if (m.Success && m.Value == END_COMMAND && !isCatched) continue;



            if (m.Success && m.Value == END_COMMAND && isCatched)
            {
                node--;
                if (node != 0) continue;

                var s = text.Substring(index, m.Index - index);
                var g = new ParentheseCommandText();
                g.Value = Parse(s);
                texts.Add(g);

                isCatched = false;
                index = m.Index + 1; //skip end command char
            }
        }

        if (index != text.Length) texts.Add(text.Substring(index, text.Length - index));

        return texts;
    }
}