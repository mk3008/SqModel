using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
	public List<SelectColumn> ParseSelectColumns()
	{
		var lst = new List<SelectColumn>();

		var fn = () =>
		{
			ReadSkipSpaces();
			lst.Add(ParseSelectColumn());
			ReadSkipSpaces();
			if (PeekOrDefault() != ',') return false;
			Read();
			return true;
		};

		while (fn()) { };
		return lst;
	}

	public SelectColumn ParseSelectColumn()
	{
		//(,)table_name.column_name(\s|,)

		var col = new SelectColumn();

		ReadSkipSpaces();
		var s = ReadUntil(" .,");

		var cn = PeekOrDefault();
		if (cn == '.')
		{
			col.TableName = s;
			Read();
			col.ColumnName = ReadUntil(" ,");
			if (PeekOrDefault() != ',') col.AliasName = ParseColumnAliasOrDefault() ?? col.ColumnName;
		}
		else if (cn == ',')
		{
			col.ColumnName = s;
		}
		else
		{
			col.ColumnName = s;
			if (PeekOrDefault() != ',') col.AliasName = ParseColumnAliasOrDefault() ?? col.ColumnName;
		}

		return col;
	}
}
