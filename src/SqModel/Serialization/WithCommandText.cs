using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class WithCommandText : ICommandText
{
    public static string COMMAND { get; } = "with";

    public string Separator { get; set; } = String.Empty;

    public ICommandText Value { get; set; } = new PlainCommandText();
    
    public string InnerText() => Value.FullText();

    public string FullText() => $"{COMMAND}{Separator}{Value.FullText()}";

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