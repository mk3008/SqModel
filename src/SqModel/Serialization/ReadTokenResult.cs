using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class ReadTokenResult
{
    public string Token { get; set; } = string.Empty;

    public string NextToken { get; set; } = string.Empty;

    public char? NextChar { get; set; } = null;
}
