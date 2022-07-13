using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public interface IText
{
    IEnumerable<string> GetValues();
}

//public class ParenthesesText: IText
//{
//    public string OpenSymbol { get; set; } = string.Empty;

//    public string CloseSymbol { get; set; } = string.Empty;

//    public List<IText> InnerTexts { get; set; } = new();

//    public IEnumerable<string> GetValues()
//    {
//        yield return OpenSymbol;

//        foreach (var item in InnerTexts)
//        {
//            foreach (var x in item.GetValues())
//            {
//                yield return x;
//            }     
//        }

//        yield return CloseSymbol;   
//    }
//}

//public class PlainText : IText
//{
//    public string Value { get; set; } = string.Empty;

//    public IEnumerable<string> GetValues()
//    {
//        yield return Value; 
//    }
//}
