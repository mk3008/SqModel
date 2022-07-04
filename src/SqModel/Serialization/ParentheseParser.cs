using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Serialization;


public static class ParentheseParser
{
    private static string PATTERN = $@"(?<prefix>.*?)(?<start>\()(?<value>(?>[^()]|(?<p>)\(|(?<-p>)\))*)(?<end>\))(?<sufix>[\s\S]*?$)";

    public static ICommandText Parse(string text)
    {

        var texts = new CommandTextCollection();

        var m = Regex.Match(text, PATTERN);

        if (!m.Success)
        {
            texts.Add(text);
            return texts;
        }

        if (m.Groups["prefix"].Value != String.Empty)
        {
            texts.Add(m.Groups["prefix"].Value);
        }

        texts.Add(new ParentheseCommandText()
        {
            Value = Parse(m.Groups["value"].Value)
        }); 

        if (m.Groups["sufix"].Value != String.Empty)
        {
            texts.Add(Parse(m.Groups["sufix"].Value));
        }

        return texts;
    }
}