﻿using SqModel.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Command;

public class CommandValue : ICommand
{
    /// <summary>
    /// Value name.
    /// </summary>
    public string CommandText { get; set; } = string.Empty;

    public Dictionary<string, object>? Parameters { get; set; } = null;

    public string Conjunction { get; set; } = String.Empty;

    public void AddParameter(string name, object value)
    {
        Parameters ??= new();
        Parameters.Add(name, value);
    }

    public Query ToQuery()
    {
        var q = new Query() { CommandText = CommandText, Parameters = Parameters ?? new() };
        q = q.InsertToken(Conjunction);
        return q;
    }
}
