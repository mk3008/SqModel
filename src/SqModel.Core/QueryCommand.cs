using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core;

public class QueryCommand
{
    public QueryCommand(string command, IDictionary<string, object?> prm)
    {
        CommandText = command;
        Parameters = prm;
    }

    public string CommandText { get; init; }

    public IDictionary<string, object?> Parameters { get; init; }
}
