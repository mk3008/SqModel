using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class Command
{
    public Command(string command, bool isSymbol = false, bool allowTrim = true)
    {
        CommandText = command;
        IsSymbol = isSymbol;
        AllowTrim = allowTrim;
    }

    public string CommandText { get; set; } = string.Empty;

    public bool IsSymbol { get; set; } = false;

    public bool AllowTrim { get; set; } = true;
}
