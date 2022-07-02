using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;


public class PlainCommandText : ICommandText
{
    public string Value { get; set; } = string.Empty;

    public string InnerText() => Value;

    public string FullText() => Value;

    public IEnumerable<string> GetValues()
    {
        yield return Value;
    }
    public IEnumerable<ICommandText> GetCommandTexts()
    {
        yield return this;
    }

    public ICommandText Segmentation(Func<string, ICommandText> parser)
    {
        var t = new CommandTextCollection();
        t.Add(parser(Value));
        if (t.GetCommandTexts().Count() == 1) return this;
        return t;
    }
}