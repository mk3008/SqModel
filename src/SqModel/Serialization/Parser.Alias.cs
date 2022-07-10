using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

partial class Parser
{
    public SelectColumn ParseSelectColumn()
    {
        //(,)table_name.column_name(\s|,)

        var col = new SelectColumn();

        ReadSkipSpaces();
        var s = ReadUntil(" \t\r\n.,");
     
        var cn = PeekOrDefault();
        if (cn == '.')
        {
            col.TableName = s;
            Read();
            col.ColumnName = ReadUntil(" \t\r\n,");
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

    public string? ParseTableAliasOrDefault()
    {
        //(table_name) as A 
        //(table_name) A
        //(table_name) (inner|left|right|cross|where|group|order)

        return Parse("as", new[] { "inner", "left", "right", "cross", "where", "grouo", "order" });
    }

    public string? ParseColumnAliasOrDefault()
    {
        //(column_name) as A 
        //(column_name) A
        //(column_name) (,|from)

        return Parse("as", new[] { ",", "from" });
    }

    public string? Parse(string command, IEnumerable<string> nextcommands, string splitters = " ,\r\n\t;")
    {
        ReadSkipSpaces();

        //select <column> [as] [<text>], ..., <column> [as] [<text>] from <table>

        var untilChars = (command == String.Empty) ? splitters : $"{splitters}{command.ToCharArray()[0]}";
        nextcommands.Select(x => x.ToCharArray()[0]).ToList().ForEach(x => untilChars += x);

        var sb = new StringBuilder();
        var fn = () =>
        {
            var s = ReadUntil(untilChars);
            sb.Append(s);

            var c = PeekOrDefault();
            if (c == null || splitters.Contains(c.Value))
            {
                return sb.ToString();
            }
            else if (s == string.Empty)
            {
                BeginTransaction();
                var tmp = ReadKeywordOrDefault(command, nextcommands);
                if (tmp == command)
                {
                    Commit();
                    ReadSkipSpaces();
                    return ReadUntil(splitters);
                }
                else if (nextcommands.Contains(tmp))
                {
                    RollBack();
                    return null;
                }
                else
                {
                    RollBack();
                }
            }

            sb.Append(ReadUntilSpace());
            return sb.ToString();
        };

        return fn();
    }
}
