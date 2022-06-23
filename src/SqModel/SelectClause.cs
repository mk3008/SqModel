using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

/// <summary>
/// Select column clause.
/// Define a list of columns to get.
/// </summary>
public class SelectClause
{
    public bool IsDistinct { get; set; } = false;

    public List<ColumnClause> ColumnClauses { get; set; } = new();

    public List<string> GetColumnNames()
    {
        var lst = new List<string>();
        
        ColumnClauses.ForEach(x => lst.Add(x.GetName()));

        return lst;
    }

    private List<Query> GetColumnQueries()
    {
        var lst = new List<Query>();
        
        lst.AddRange(ColumnClauses.Select(x => x.ToQuery()));

        return lst;
    }

    public Query ToQuery()
    {
        var q = GetColumnQueries().ToList().ToQuery(", ");
        var distinct = (IsDistinct) ? "distinct " : "";
        q.CommandText = $"select {distinct}{q.CommandText}";
 
        return q;
    }
}