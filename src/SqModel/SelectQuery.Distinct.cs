using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQueryDistinct
{
    public static void Distinct(this SelectQuery source, bool isDiscintct = true)
    {
        source.SelectClause.IsDistinct = isDiscintct;
    }
}
