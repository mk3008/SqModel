using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class CommandTextCollection : ICommandText
{
    public List<ICommandText> Values { get; set; } = new();

    public void Add(string text) => Values.Add(new PlainCommandText() { Value = text });

    public void Add(ICommandText text) => Values.Add(text);

    public string InnerText() => Values.Select(x => x.InnerText()).ToList().ToString("");

    public string FullText() => Values.Select(x => x.FullText()).ToList().ToString("");

    public IEnumerable<string> GetValues()
    {
        foreach (var item in Values)
        {
            foreach (var x in item.GetValues())
            {
                yield return x;
            }
        }
    }

    public IEnumerable<ICommandText> GetCommandTexts()
    {
        foreach (var item in Values)
        {
            foreach (var x in item.GetCommandTexts())
            {
                yield return x;
            }
        }
    }

    public ICommandText Segmentation(Func<string, ICommandText> parser)
    {
        var lst = new CommandTextCollection();
        Values.ForEach(x => lst.Add(x.Segmentation(parser)));
        return lst;
    }
}
