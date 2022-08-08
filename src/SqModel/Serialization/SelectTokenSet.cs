using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class SelectTokenSet
{
    public List<SelectColumnTokenSet> Columns { get; set; } = new();

    public SelectTableTokenSet Table { get; set; } = new();
}
