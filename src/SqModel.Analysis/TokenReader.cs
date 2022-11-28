//using SqModel.Analysis.Extensions;
//using SqModel.Core;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;

//namespace SqModel.Analysis;

//public partial class TokenReader
//{
//    public TokenReader(string text)
//    {
//        Text = text;
//        Reader = new StringReader(Text);
//    }

//    public string Text { get; init; }

//    private StringReader Reader { get; init; }

//    public Action<string>? Logger { get; set; }

//    internal char? PeekOrDefault()
//    {
//        var i = Reader.Peek();
//        if (i.IsEof()) return null;
//        return (char)i;
//    }

//    private void SkipSpace()
//    {
//        var c = PeekOrDefault();
//        while (!c.IsSpace())
//        {
//            Read();
//            c = PeekOrDefault();
//        }
//    }

//    private void SkipUntilLineEnd()
//    {
//        var c = PeekOrDefault();

//        while (c != null)
//        {
//            if (c.Value == '\r')
//            {
//                Read();
//                c = PeekOrDefault();
//                if (c != null && c.Value == '\n') Read();
//                break;
//            }
//            if (c.Value == '\n')
//            {
//                Read();
//                break;
//            }
//            Read();
//            c = PeekOrDefault();
//        }
//        return;
//    }
//}