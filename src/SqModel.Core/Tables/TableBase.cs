using SqModel.Core.Clauses;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Tables;

public abstract class TableBase : ITable
{
    public abstract string GetCommandText();

    public virtual string GetDefaultName() => string.Empty;
}