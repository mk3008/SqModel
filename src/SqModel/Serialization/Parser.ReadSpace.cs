using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
	public string ReadUntilCrLf()
	{
		var digit = (char c) =>
		{
			if (c == '\n')
			{
				Read();
				return false;
			}
			if (c == '\r')
			{
				Read();
				var next = PeekOrDefault();
				if (next.HasValue && next.Value == '\n')
				{
					Read();
					return false;
				}
				return false;
			}
			return true;
		};
		return ReadWhile(digit);
	}

	public string ReadWhileSpace()
	{
		var digit = (char c) => c.IsSpace();
		return ReadWhile(digit);
	}

	public string ReadUntilSpaceOrSymbol()
	{
		var digit = (char c) => !c.IsSpace() && !c.IsSymbol();
		return ReadWhile(digit);
	}

	public string ReadWhile(Func<char, bool> digit)
	{
		var s = new StringBuilder();
		var fn = () =>
		{
			var c = PeekOrDefault();
			if (c == null) return false;
			if (!digit(c.Value)) return false;
			s.Append(Read());
			return true;
		};
		while (fn()) { }
		return s.ToString();
	}
}
