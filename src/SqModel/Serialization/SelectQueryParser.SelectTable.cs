using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SelectQueryParser
{
	//public ReadTokenResult ParseSelectTable()
	//{
	//	// (schema_name.)table_name( as alias)

	//	var tbl = new SelectTable();

	//	ReadSkipSpaces();
	//	var s = ReadUntil(" .");

	//	var cn = PeekOrDefault();
	//	if (cn == '.')
	//	{
	//		tbl.Schema = s;
	//		Read();
	//		tbl.TableName = ReadUntilSpace();
	//		tbl.AliasName = ParseTableAliasOrDefault() ?? string.Empty;
	//	}
	//	else
	//	{
	//		tbl.TableName = s;
	//		tbl.AliasName = ParseTableAliasOrDefault() ?? string.Empty;
	//	}

	//	return tbl;
	//}
}
