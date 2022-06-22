using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;


public interface IParameterCollection {
    Dictionary<string, object> Parameters { get; set; }
}

public static class IParameterCollectionAddParameter
{
    public static IParameterCollection AddParameter(this IParameterCollection source, string key, object value)
    {
        source.Parameters[key] = value;
        return source;
    }
}
