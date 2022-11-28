//using SqModel.Analysis.Extensions;
//using SqModel.Core;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;

//namespace SqModel.Analysis;

//public partial class TokenReader
//{
//    private IEnumerable<char> ReadChars() => ReadChars(_ => true);

//    private IEnumerable<char> ReadChars(Func<char, bool> digit)
//    {
//        var c = PeekOrDefault();
//        while (c != null && digit(c.Value))
//        {
//            yield return Read();
//            c = PeekOrDefault();
//        }
//    }

//    private char Read()
//    {
//        var i = Reader.Read();
//        if (i.IsEof()) throw new EndOfStreamException();
//        return (char)i;
//    }
//}