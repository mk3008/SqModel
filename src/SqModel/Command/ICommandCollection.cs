using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Command;

public interface ICommandCollection<T> where T : ICommand, new()
{
    List<T> Collection { get; }
}

public static class ICommandCollectionExtension
{
    public static T Add<T>(this ICommandCollection<T> soruce) where T : ICommand, new()
    {
        var c = new T();
        soruce.Collection.Add(c);
        return c;
    }
}