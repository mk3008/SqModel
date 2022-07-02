using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class WithParser
{
    private static string PATTERN = $@"(\s|^)with\s*";

    public static ICommandText Parse(string text)
    {

        var texts = new CommandTextCollection();

        var index = 0;

        var ms = Regex.Matches(text, PATTERN, RegexOptions.IgnoreCase);

        foreach (Match m in ms)
        {
            //var mIndex = m.Index;
            //var mLenght = m.Length;

            if (m.Success)
            {
                if (m.Index != index)
                {
                    //plan text
                    //if (!m.Value.StartsWith("w"))
                    //{
                    //    mIndex++;
                    //    mLenght--;
                    //}

                    var value = text.Substring(index, m.Index - index);
                    texts.Add(value, index);
                }

                var ct = new WithCommandText();
                texts.Add(ct);
                index = m.Index + m.Length;

                //if (index >= text.Length + 1)
                //{
                var s = m.Value.Trim();
                ct.Separator = m.Value.Substring(4, m.Length - 4);
                ct.Value = new PlainCommandText() { Value = text.Substring(index, text.Length - index) };
                //};         

                index = text.Length;
                break;
            }
        }

        if (index != text.Length) texts.Add(text.Substring(index, text.Length - index), index);

        return texts;
    }
}
