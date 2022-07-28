﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public class SelectColumnTokenSet
{
    public string TableName { get; set; } = string.Empty;

    public string ColumnName { get; set; } = string.Empty;

    public string AliasName { get; set; } = string.Empty;

    public string FullName => $"{TableName}.{ColumnName}";

    public string Name => (AliasName == string.Empty) ? ColumnName : AliasName;
}