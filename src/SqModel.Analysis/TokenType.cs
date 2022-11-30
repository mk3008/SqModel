using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public enum TokenType
{
    With,
    WithText,
    Union,
    UnionKeyword,
    Select,
    SelectKeyword,
    SelectText,
    From,
    FromText,
    Where,
    WhereText,
    GroupBy,
    GroupByText,
    Having,
    HavingText,
    OrderBy,
    OrderByText,

    SelectItem,

    NumericValue,
    TextValue,
    Table,
    Column,
    Boolean,

    Operator,

    BlockCommentStart,
    BlockComment,

    LineCommentStart,
    LineComment,

    BracketStart,
    BracketValue,
    BracketEnd,

    Function,

    CaseStart,
    CaseCondition,
    CaseWhenText,
    CaseEnd,

    Alias,
    AliasName,

    InnerJoin,
    On,
    OnText

}
