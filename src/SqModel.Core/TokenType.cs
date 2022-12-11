using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core;

public enum TokenType
{
    Reserved = 0,
    Clause,
    Bracket,
    Operator,
    //AndOperator,
    //OrOperator,
    ValueSplitter, // ,
    Value,
    ValueName,
    Table,
    TableName,
    Control,
}

public enum BlockType
{
    BlockStart,
    BlockEnd,
    Splitter,
    Default,
}