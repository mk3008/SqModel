using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

partial class Parser
{
	public void ParseTableAlias(Action<string> setter)
	{
		//(table_name) as A 
		//(table_name) A
		//(table_name) (inner|left|right|cross|where|group|order)

		Parse(setter, "as", new[] { "inner", "left", "right", "cross", "where", "grouo", "order" });
	}

	public void ParseColumnAlias(Action<string> setter)
	{
		//(column_name) as A 
		//(column_name) A
		//(column_name) (,|from)

		Parse(setter, "as", new[] { "from" });
	}

	public void Parse(Action<string> setter, string command, IEnumerable<string> nextcommands, string splitters = " ,\r\n\t;")
	{
		ReadSkipSpaces();

		//select <column> [as] [<text>], ..., <column> [as] [<text>] from <table>

		var untilChars = $"{splitters}{command.ToCharArray()[0]}";
		nextcommands.Select(x => x.ToCharArray()[0]).ToList().ForEach(x => untilChars += x);

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
				var tmp = ReadKeywordOrDefault(command, nextcommands);
				if (tmp == command)
				{
					Commit();
					ReadSkipSpaces();
					setter(ReadUntil(splitters));
					return;
				}
				else if (nextcommands.Contains(tmp))
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
