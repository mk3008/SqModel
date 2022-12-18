﻿using Carbunql.Core.Extensions;
using Carbunql.Core.Values;

namespace Carbunql.Core.Clauses;

public class CommonTable : SelectableTable
{
    public CommonTable(TableBase table, string alias) : base(table, alias)
    {
    }

    public CommonTable(TableBase table, string alias, ValueCollection columnAliases) : base(table, alias, columnAliases)
    {
    }

    public MaterializedType Materialized { get; set; } = MaterializedType.Undefined;

    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in GetAliasTokens(parent)) yield return item;
        yield return Token.Reserved(this, parent, "as");

        if (Materialized != MaterializedType.Undefined)
        {
            yield return Token.Reserved(this, parent, Materialized.ToCommandText());
        }

        foreach (var item in Table.GetTokens(parent)) yield return item;
    }
}