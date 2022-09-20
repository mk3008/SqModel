using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Extension;

namespace SqModel;

public class CommonTable : IQueryable
{
    public SelectQuery Query { get; set; } = new();

    public List<string> Keywords { get; set; } = new();

    public string Name { get; set; } = string.Empty;

    public Query ToQuery()
    {
        Query.IsIncludeCte = false;
        var q = Query.ToQuery();

        if (Keywords.Any())
        {
            q.CommandText = $"{Name} as {Keywords.ToString(" ")} (\r\n{q.CommandText.InsertIndent()}\r\n)";
        }
        else
        {
            q.CommandText = $"{Name} as (\r\n{q.CommandText.InsertIndent()}\r\n)";
        }

        return q;
    }
}

public static class CommonTableExtension
{
    public static CommonTable As(this CommonTable source, string name)
    {
        source.Name = name;
        return source;
    }
}