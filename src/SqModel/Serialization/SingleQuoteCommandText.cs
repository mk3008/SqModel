using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class SingleQuoteCommandText : ICommandText
{
    public int Index { get; set; } = 0;

    public string Prefix { get; } = "'";

    public string Value { get; set; } = String.Empty;

    public string Suffix { get; set; } = "'";

    public string InnerText() => String.Empty;

    public string FullText() => $"{Prefix}{Value}{Suffix}";

    public IEnumerable<string> GetValues()
    {
        yield return Value;
    }

    public IEnumerable<ICommandText> GetCommandTexts()
    {
        yield return this;
    }

    public ICommandText Segmentation(Func<string, ICommandText> parser) => this; //segmentation ignore
}