using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public Action<string>? Logger { get; set; }

    private List<CommandString> Commands = new()
    {
        new CommandString("with" ),
        new CommandString("select"),
        new CommandString("distinct"),
        new CommandString("limit"),
        new CommandString("as"),
        new CommandString("from"),
        new CommandString("inner join"),
        new CommandString("left outer join"),
        new CommandString("left join"),
        new CommandString("right outer join"),
        new CommandString("right join"),
        new CommandString("cross join"),
        new CommandString("where"),
        new CommandString("group by"),
        new CommandString("having"),
        new CommandString("order by"),
        new CommandString("and"),
        new CommandString("or"),
        new CommandString("(", true),
        new CommandString("--", true),
        new CommandString("/*", true),
        new CommandString("'", true),
        new CommandString("\r", true),
        new CommandString("\n", true),
        new CommandString(",", true),
    };
    public ReadCommandResult ReadUntilCommand()
    {
        return ReadUntilCommand(Commands);
    }

    public ReadCommandResult ReadUntilCommand(IEnumerable<CommandString> commands)
    {
        Logger?.Invoke("start ReadUntilCommand");

        var value = new StringBuilder();
        var command = new StringBuilder();

        var initdic = () => commands.ToList().ToDictionary(x => x, y => y.Command);

        var dic = initdic();
        var isPrevMatch = false;
        var isPrevSpace = true;

        ReadSkipSpaces();

        var fn = () =>
        {
            var cn = PeekOrDefault();
            if (cn == null)
            {
                //cache clear
                value.Append(command.ToString());
                command.Clear();
                return false;
            }

            var c = cn.Value;

            var q = dic.Where(x => x.Value.IsFirstChar(c));

            if (!q.Any())
            {
                if (isPrevMatch)
                {
                    //not command
                    isPrevMatch = false;
                    dic = initdic();
                    value.Append(command.ToString());
                    command.Clear();
                }
                value.Append(Read());
            }
            else
            {
                if (isPrevSpace) isPrevMatch = true;
                if (isPrevMatch)
                {
                    //continue command cache 
                    dic = q.ToDictionary(x => x.Key, y => y.Value.Substring(1, y.Value.Length - 1));
                    command.Append(Read());
                }
                else
                {
                    //not word
                    q = q.Where(x => x.Key.IsSymbol);
                    if (q.Any())
                    {
                        //but symbol
                        dic = q.ToDictionary(x => x.Key, y => y.Value.Substring(1, y.Value.Length - 1));
                        command.Append(Read());
                    }
                    else
                    {
                        value.Append(Read());
                    }
                }
            }

            if (Logger != null)
            {
                var msg = $"capture:{c}, peek:{PeekOrDefault()}, value:{value.ToString()}, command:{command.ToString()}";
                Logger.Invoke(msg);
            }

            if (dic.Any() && dic.Count == 1)
            {
                if (commands.ToList().Where(x => x.IsSymbol && x.Command.ToLower() == command.ToString()).Any()) return false;
                if (PeekOrDefault().IsSpace()) return false;
                if (PeekOrDefault == null) return false;
            }

            isPrevSpace = c.IsSpace();
            return true;
        };

        while (fn()) { }

        var result = new ReadCommandResult()
        {
            Command = command.ToString().ToLower(),
            IsSuccess = (command.Length == 0) ? false : true,
            Value = value.ToString().TrimEnd(" \t\r\n".ToCharArray()),
        };

        if (Logger != null)
        {
            var msg = $"result.IsSuccess:{result.IsSuccess}, result.value:{result.Value}, result.Command:{result.Command}";
            Logger.Invoke(msg);
        }

        Logger?.Invoke("end   ReadUntilCommand");

        return result;
    }
}

public class ReadCommandResult
{
    public bool IsSuccess { get; set; } = false;

    public string Value { get; set; } = string.Empty;

    public string Command { get; set; } = string.Empty;
}
