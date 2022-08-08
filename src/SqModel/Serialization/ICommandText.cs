using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public interface ICommandText
{
    ICommandText Segmentation(Func<string, ICommandText> parser);

    IEnumerable<string> GetValues();

    IEnumerable<ICommandText> GetCommandTexts();

    string FullText();

    string InnerText();
}