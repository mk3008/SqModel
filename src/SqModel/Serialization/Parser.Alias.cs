using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

partial class Parser
{


	public void ParseColumnAlias(Action<string> setter)
	{
		//(column_name or table_name) as A 
		//(column_name or table_name) A
		//(column_name or table_name),
		//(column_name)\s

		Parse(setter, "as", "from");
	}

	public void Parse(Action<string> setter, string command, string nextcommand, string splitters = " ,\r\n\t;")
	{
		ReadSkipSpaces();

		//select <column> [as] [<text>], ..., <column> [as] [<text>] from <table>

		var untilChars = $"{splitters}{command.ToCharArray()[0]}{nextcommand.ToCharArray()[0]}";

		var sb = new StringBuilder();
		var fn = () =>
		{
			var s = ReadUntil(untilChars);
			sb.Append(s);

			var c = PeekOrDefault();
			if (c == null || splitters.Contains(c.Value))
			{
				setter(sb.ToString());
				return;
			}
			else if (s == string.Empty)
			{
				BeginTransaction();
				var tmp = ReadKeywordOrDefault(new[] { command, nextcommand });
				if (tmp == command)
				{
					Commit();
					ReadSkipSpaces();
					setter(ReadUntil(splitters));
					return;
				}
				else if (tmp == nextcommand)
				{
					RollBack();
					return;
				}
				else
				{
					RollBack();
				}
			}

			sb.Append(ReadUntilSpace());
			setter(sb.ToString());
		};

		fn();
	}
}
