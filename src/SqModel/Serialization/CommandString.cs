using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class CommandString
{
    public CommandString(string command, bool isSymbol = false)
    {
        Command = command;
        IsSymbol = isSymbol;
    }

    public string Command { get; set; } = string.Empty;

    public bool IsSymbol { get; set; } = false;
}
