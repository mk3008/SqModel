using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class WithParser
{
    private static string PATTERN = $@"(?<prefix>\s.*|^)(?<keyword>with)(?<separator>\s*)(?<value>.*)(?<sufix>select.*)";

    public static ICommandText Parse(string text)
    {

        var texts = new CommandTextCollection();
        var m = Regex.Match(text, PATTERN, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (!m.Success)
        {
            texts.Add(text);
            return texts;
        }

        if (m.Groups["prefix"].Value.Length != 0)
        {
            texts.Add(m.Groups["prefix"].Value);
        }

        texts.Add(new WithCommandText()
        {
            Value = new PlainCommandText() { Value = m.Groups["value"].Value },
            Separator = m.Groups["separator"].Value
        });

        texts.Add(m.Groups["sufix"].Value);

        return texts;
    }
}
