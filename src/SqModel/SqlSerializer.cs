using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel;

public class SqlSerializer
{
    private static string SINGLE_LINE_COMMENT_COMMAND = "--";
    private static string MULTIPLE_LINE_COMMENT_START_COMMAND = "/*";
    private static string MULTIPLE_LINE_COMMENT_END_COMMAND = "*/";
    private static string[] NEWL_LINE_COMMANDS = new string[] { "\r", "\n" };

    public static string RemoveComment(string sql)
    {
        var lst = new List<string>();
        lst.Add(SINGLE_LINE_COMMENT_COMMAND);
        lst.Add(MULTIPLE_LINE_COMMENT_START_COMMAND.Replace("*", @"\*"));
        lst.Add(MULTIPLE_LINE_COMMENT_END_COMMAND.Replace("*", @"\*"));
        lst.AddRange(NEWL_LINE_COMMANDS);
        var pattern = $"({lst.ToString("|")})";
        var mc = Regex.Matches(sql, pattern, RegexOptions.Multiline);

        var sb = new StringBuilder();
        var index = 0;
        var isMultiple = false;
        var isSingle = false;
        foreach (Match m in mc)
        {
            if (m.Success && !isMultiple && !isSingle)
            {
                if (!NEWL_LINE_COMMANDS.Contains(m.Value))
                {
                    sb.Append(sql.Substring(index, m.Index - index));
                }
            }

            if (m.Value == MULTIPLE_LINE_COMMENT_START_COMMAND && !isMultiple  && !isSingle )
            {
                isMultiple = true;
            }
            else if (m.Value == SINGLE_LINE_COMMENT_COMMAND && !isMultiple  && !isSingle)
            {
                isSingle = true;
            }
            else if (m.Value == MULTIPLE_LINE_COMMENT_END_COMMAND && isMultiple)
            {
                isMultiple = false;
                index = m.Index + m.Length;
            }
            else if (NEWL_LINE_COMMANDS.Contains(m.Value) && isSingle)
            {
                isSingle = false;
                index = m.Index;
            }
        }

        if (!isMultiple && !isSingle) sb.Append(sql.Substring(index, sql.Length - index));

        return sb.ToString();
    }

    public static string RemoveMultipleLineComment(string sql) => Regex.Replace(sql, @"^(?!\-\-)/\*[\s\S]*?\*/", "", RegexOptions.Multiline);

    public static string RemoveSingleLineComment(string sql) => Regex.Replace(sql, @"\-\-.*", "");
}
