using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public SelectQuery Parse()
    {
        ReadSkipSpaces();

        var s = ReadCommandOrDefault(new[] { "select" });
        if (s == null) throw new Exception();

        var cols = ParseSelectColumns();

        s = ReadCommandOrDefault(new[] { "from" });
        if (s == null) throw new Exception();

        var tbl = ParseSelectTable();

        var q = new SelectQuery();
        cols.ForEach(x => q.Select(x.FullName, x.Name));
        q.From(tbl.FullName, tbl.Name);
        return q;
    }
}
