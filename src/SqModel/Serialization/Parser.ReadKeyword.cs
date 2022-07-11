﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    private List<string> Keywords = new()
    {
        "with",
        "select",
        "distinct",
        "limit",
        "as",
        "from",
        "inner join",
        "left outer join",
        "left join",
        "right outer join",
        "right join",
        "cross join",
        "where",
        "group by",
        "habing",
        "order by"
    };

    public string ReadKeywordOrDefault(string command, IEnumerable<string> nextcommands)
    {
        var lst = nextcommands.ToList();
        lst.Add(command);
        return ReadKeywordOrDefault(lst);
    }

    public string ReadKeywordOrDefault(IEnumerable<string> commands)
    {
        var sb = new StringBuilder();
        var lst = new List<string>();
        lst.AddRange(commands);

        var fn = () =>
        {
            var cn = PeekOrDefault();
            if (cn == null) return false;

            var c = cn.Value;
            lst = lst.Where(x => x.IsFirstChar(c)).Select(x => x.Substring(1, x.Length - 1)).ToList();
            if (!lst.Any()) return false;

            sb.Append(Read());
            if (PeekOrDefault().IsSpace()) return false;

            return true;
        };

        while (fn()) { }

        var s = sb.ToString().ToLower();
        if (!PeekOrDefault().IsSpace() || !commands.Contains(s, x => x.ToLower()))
        {
            return String.Empty;
        }

        return s;
    }
}