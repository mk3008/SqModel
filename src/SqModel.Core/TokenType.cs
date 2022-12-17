using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core;

public enum TokenType
{
    Default,
    Reserved,
}

public enum BlockType
{
    Default,
    Start,
    End,
    Split,
}