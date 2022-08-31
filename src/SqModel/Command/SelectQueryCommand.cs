using SqModel;
using SqModel.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Command;

public class SelectQueryCommand : ICommand
{
    public SelectQuery? Query { get; set; } = null;

    public string Conjunction { get; set; } = String.Empty;

    public void AddParameter(string name, object value)
        => throw new NotSupportedException();

    public Query ToQuery()
    {
        if (Query == null) throw new InvalidProgramException();
        Query.IsincludeCte = false;
        return Query.ToQuery().DecorateBracket().InsertToken(Conjunction);
    }
}