using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class ParentheseCommandText : ICommandText
{
    public string Prefix { get; } = "(";
    public ICommandText Value { get; set; } = new PlainCommandText();
    public string Suffix { get; } = ")";

    public string InnerText() => Value.FullText();

    public string FullText() => $"{Prefix}{Value.FullText()}{Suffix}";

    public IEnumerable<string> GetValues()
    {
        return Value.GetValues();
    }

    public IEnumerable<ICommandText> GetCommandTexts()
    {
        yield return this;
    }

    public ICommandText Segmentation(Func<string, ICommandText> parser)
    {
        var t = new CommandTextCollection();
        Value.GetCommandTexts().ToList().ForEach(x => t.Add(x.Segmentation(parser)));
        if (t.GetCommandTexts().Count() == 1) return this;
        return t;
    }
}


