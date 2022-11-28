using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core;

public enum RelationTypes
{
    Undefined = 0,
    From = 1,
    Inner = 2,
    Left = 3,
    Right = 4,
    Cross = 5,
}