using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public TableClause ParseTableClause()
    {
        return FromClauseParser.Parse(this);
    }
}
