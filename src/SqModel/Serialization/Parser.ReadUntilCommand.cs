using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public Action<string>? Logger { get; set; }

    private List<Command> Commands = new()
    {
        new Command("with" ),
        new Command("select"),
        new Command("distinct"),
        new Command("limit"),
        new Command("as"),
        new Command("from"),
        new Command("inner join"),
        new Command("left outer join"),
        new Command("left join"),
        new Command("right outer join"),
        new Command("right join"),
        new Command("cross join"),
        new Command("where"),
        new Command("group by"),
        new Command("having"),
        new Command("order by"),
        new Command("and"),
        new Command("or"),
        new Command("(", true),
        new Command("--", true, false),
        new Command("/*", true, false),
        new Command("'", true, false),
        new Command("\r", true),
        new Command("\n", true),
        new Command(",", true),
    };
    public ReadCommandResult ReadUntilCommand()
    {
        return ReadUntilCommand(Commands);
    }

    public ReadCommandResult ReadUntilCommand(IEnumerable<Command> commands)
    {
        Logger?.Invoke("start ReadUntilCommand");

        var value = new StringBuilder();
        var command = new StringBuilder();
        var sufix = new StringBuilder();

        var initdic = () => commands.ToList().ToDictionary(x => x, y => y.CommandText);

        var dic = initdic();
        var isPrevMatch = false;
        var isPrevSpace = true;

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
                var isHit = false;
                if (commands.ToList().Where(x => x.IsSymbol && x.CommandText.ToLower() == command.ToString()).Any()) isHit = true;
                else if (PeekOrDefault().IsSpace()) isHit = true;
                else if (PeekOrDefault() == null) isHit = true;
                if (isHit) return false;
            }

            isPrevSpace = c.IsSpace();
            return true;
        };

        while (fn()) { }

        var c = commands.Where(x => x.CommandText == command.ToString().ToLower()).FirstOrDefault();
        var result = new ReadCommandResult();
        if (c != null) result.Command = c;
        result.Value = (result.Command.AllowTrim) ? value.ToString().TrimEnd(" \t\r\n".ToCharArray()) : value.ToString();
        
        if (Logger != null)
        {
            var msg = $"result.value:{result.Value}, result.Command:{result.Command?.CommandText}";
            Logger.Invoke(msg);
        }

        Logger?.Invoke("end   ReadUntilCommand");

        return result;
    }
}

public class ReadCommandResult
{
    public string Value { get; set; } = string.Empty;

    public Command Command { get; set; } = new Command("", false, true);

    public string SufixSymbol { get; set; } = string.Empty;
}
