﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public class SyntaxException : Exception
{
    public SyntaxException(string? message) : base(message) { }
}