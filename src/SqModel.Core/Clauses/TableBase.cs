using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Clauses;

public abstract class TableBase : IQueryCommand, IQueryParameter
{
    public abstract string GetCommandText();

    public virtual string GetDefaultName() => string.Empty;

    public virtual IDictionary<string, object?> GetParameters() => EmptyParameters.Get();
}