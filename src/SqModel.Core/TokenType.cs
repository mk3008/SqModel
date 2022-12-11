using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core;

public enum TokenType
{
    Reserved = 0,
    SelectClauseStart,
    SelectClauseEnd,
    ValueStart,
    ValueEnd,
    FromClauseStart,
    FromClauseEnd,
    RelationStart,
    RelationEnd,
    RelationConditionStart,
    RelationConditionEnd,
    TableStart,
    TableEnd,
    BracketStart,
    BracketEnd,
    AndOperator,
    OrOperator,
    ValueSplitter, // ,
    Value,
    ValueName,
    Table,
    TableName,
    Operator
}

public enum BlockType
{
    BlockStart,
    BlockEnd,
    Splitter,
    Default,
}