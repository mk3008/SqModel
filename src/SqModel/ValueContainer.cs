using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ValueContainer : IValueContainer
{
    public IValueClause? Command { get; set; }

    public string ColumnName
    {
        set { return; }
    }

    public string Name
    {
        set { return; }
    }

    public virtual Query ToQuery()
    {
        if (Command == null) throw new InvalidProgramException();
        return Command.ToQuery();
    }
}