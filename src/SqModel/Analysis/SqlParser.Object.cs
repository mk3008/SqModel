using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public static SelectQuery Parse(object obj, string parametercmd = ":", Func<string, string>? nameconverter = null, Func<PropertyInfo, bool>? propfilter = null)
    {
        var conv = nameconverter;
        conv ??= x => x;

        var filter = propfilter;
        filter ??= _ => true;

        var sq = new SelectQuery();

        obj.GetType().GetProperties().Where(x => filter(x)).ToList().ForEach(x =>
        {
            var key = $"{parametercmd}{x.Name}".ToLower();
            var val = x.GetValue(obj);
            sq.Select.Add().Value(key).AddParameter(key, val).As($"{conv(x.Name)}");
        });

        return sq;
    }

    public static SelectQuery Parse<T>(Func<string, string>? nameconverter = null, Func<PropertyInfo, bool>? propfilter = null)
    {
        var conv = nameconverter;
        conv ??= x => x.ToLower();

        var filter = propfilter;
        filter ??= _ => true;

        var sq = new SelectQuery();
        var t = sq.From(conv(typeof(T).Name)).As("t");
        typeof(T).GetProperties().Where(x => filter(x)).ToList().ForEach(x =>
        {
            sq.Select.Add().Column(t, conv(x.Name)).As(x.Name.ToLower());
        });

        return sq;
    }
}
