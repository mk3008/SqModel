using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class NamelessItem : IValueContainer
{
    public IValueClause? Command { get; set; }

    public string ColumnName { get; set; } = String.Empty;

    public string Name { get; set; } = String.Empty;

    public Query ToQuery()
    {
        if (Command == null) throw new InvalidProgramException();
        var q = Command.ToQuery();
        return q;
    }
}