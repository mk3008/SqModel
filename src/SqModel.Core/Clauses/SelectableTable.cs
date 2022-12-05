﻿using SqModel.Core.Extensions;
using SqModel.Core.Values;
using System.ComponentModel;

namespace SqModel.Core.Clauses;

public class SelectableTable : IQueryable, ISelectable
{
    public SelectableTable(ITable table, string alias)
    {
        Table = table;
        Alias = alias;
    }
    public SelectableTable(ITable table, string alias, ValueCollection columnAliases)
    {
        Table = table;
        Alias = alias;
        ColumnAliases = columnAliases;
    }

    public ITable Table { get; init; }

    public Dictionary<string, object?>? Parameters { get; set; }

    public string Alias { get; init; }

    public ValueCollection? ColumnAliases { get; init; }

    private string GetAliasCommand()
    {
        /*
         * alias(col1, col2, col3)
         */
        if (string.IsNullOrEmpty(Alias)) return string.Empty;
        if (ColumnAliases == null || !ColumnAliases.Any())
        {
            if (Alias == Table.GetDefaultName()) return string.Empty;
            return Alias;
        }

        return Alias + "(" + ColumnAliases.GetCommandText() + ")";
    }

    public string GetCommandText()
    {
        /*
         * query as alias(col1, col2, col3) 
         */

        var query = Table.GetCommandText();
        var alias = GetAliasCommand();
        if (string.IsNullOrEmpty(alias)) return query;
        return $"{query} as {alias}";
    }

    public IDictionary<string, object?> GetParameters()
    {
        return Parameters ?? EmptyParameters.Get();
    }
}