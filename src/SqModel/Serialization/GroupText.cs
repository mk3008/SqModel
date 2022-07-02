using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class GroupText : IText
{
    public int Index { get; set; } = 0;

    public string Prefix { get; set; } = string.Empty;
    public IText Value { get; set; } = new PlainText();
    public string Suffix { get; set; } = string.Empty;

    public string InnerText() => Value.FullText();

    public string SegmentationText() => FullText().Length.Space();

    public string FullText() => $"{Prefix}{Value.FullText()}{Suffix}";

    public IEnumerable<string> GetValues()
    {
        return Value.GetValues();
    }

    public IText Segmentation(Func<string, IText> parser)
    {
        Value = Value.Segmentation(parser);
        return this;
    }

    public IEnumerable<IText> GetTexts()
    {
        yield return this;
    }
}

public class PlainText : IText
{
    public int Index { get; set; } = 0;

    public string Value { get; set; } = string.Empty;

    public string InnerText() => Value;

    public string SegmentationText() => Value;

    public string FullText() => Value;

    public IEnumerable<string> GetValues()
    {
        yield return Value;
    }
    public IEnumerable<IText> GetTexts()
    {
        yield return this;
    }

    public IText Segmentation(Func<string, IText> parser)
    {
        var text1 = parser(Value);

        var text2 = parser(text1.SegmentationText());

        if (text1.SegmentationText() == text2.SegmentationText()) return text1;

        var merged = new TextCollection();

        //var index

        //foreach (var t1 in text1.GetTexts())
        //{
        //    text1.Select (x => text1)


        //    foreach (var t2 in text2.GetTexts())
        //    {

        //    }
        //}

        var tmp1 = string.Empty;
        var lst1 = text1.GetTexts().ToList();
        var tmp2 = string.Empty;
        var lst2 = text2.GetTexts().ToList();

        foreach (var t2 in lst2)
        {
            if (t2 is PlainText)
            {
                merged.Add(t2);
            }
            else
            {
                var s = Value.Substring(t2.Index + 1, t2.InnerText().Length);
                var g = new GroupText() { Prefix= "(", Suffix= ")" };
                g.Value = parser(s);
                merged.Add(g);
            }
        }
        
        return text2;
    }
}

public class TextCollection : IText
{
    public int Index { get; set; } = -1;

    public List<IText> Values { get; set; } = new();

    public void Add(string text, int index) => Values.Add(new PlainText() { Value = text, Index = index });

    public void Add(IText text) => Values.Add(text);

    public string InnerText() => Values.Select(x => x.InnerText()).ToList().ToString("");

    public string SegmentationText() => Values.Select(x => x.SegmentationText()).ToList().ToString("");

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

    public IEnumerable<IText> GetTexts()
    {
        foreach (var item in Values)
        {
            foreach (var x in item.GetTexts())
            {
                yield return x;
            }
        }
    }

    public IText Segmentation(Func<string, IText> parser)
    {
        var lst = new TextCollection();
        Values.ForEach(x => lst.Add(x.Segmentation(parser)));
        return lst;
    }
}

public interface IText
{
    int Index { get; }

    IText Segmentation(Func<string, IText> parser);

    IEnumerable<string> GetValues();

    IEnumerable<IText> GetTexts();

    string FullText();

    string InnerText();

    string SegmentationText();
}

