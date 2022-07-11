using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class SelectTable
{
    public string Schema { get; set; } = string.Empty;

    public string TableName { get; set; } = string.Empty;

    public string AliasName { get; set; } = string.Empty;
}
