using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

internal static class intExtensions
{
    public static void ForEach(this int source, Action<int> action)
    {
        for (int i = 0; i < source; i++) action(i);
    }

    public static string ToSpaceString(this int source)
    {
        var space = string.Empty;
        source.ForEach(x => space += " ");
        return space;
    }
}
