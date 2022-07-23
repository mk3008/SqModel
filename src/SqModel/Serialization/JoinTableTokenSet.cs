using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class JoinTableTokenSet
{
    public string JoinToken { get; set; } = string.Empty;

    public SelectTableTokenSet TableToken { get; set; } = new();

    public string Condition { get; set; } = string.Empty;
}
