using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core;

public interface IQueryParameter : IQueryCommand
{
    IDictionary<string, object?> GetParameters();
}