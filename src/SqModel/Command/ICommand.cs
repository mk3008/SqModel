using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Command;

public interface ICommand : IQueryable
{
    string Conjunction { get; set; }

    void AddParameter(string name, object value);
}
public static class ICommandExtension
{
    public static ICommand Conjunction(this ICommand source, string sign)
    {
        source.Conjunction = sign;
        return source;
    }

    public static ICommand Parameter(this ICommand source, string key, object value)
    {
        source.AddParameter(key, value);
        return source;
    }
}