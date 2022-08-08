using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SelectQueryParser
{
    public ReadTokenResult ParseSelectColumn(SelectTokenSet s)
    {
        ReadWhileSpace();

        var c = new SelectColumnTokenSet();
        var tokens = new[] { ".", "as", ",", "from", ";"}.ToList();

        var r = ReadUntilToken(tokens);

        var gettokens = (string token) =>
        {
            return tokens.Where(x => tokens.IndexOf(x) > tokens.IndexOf(token));
        };

        var read = (string? ts) =>
        {
            ReadWhileSpace();
            r = ReadUntilToken((ts == null) ? tokens : gettokens(ts));
            return true;
        };

        var fn = () =>
        {
            if (r.NextToken == ".")
            {
                c.TableName = r.Token.TrimEndSpace();
                return read(r.NextToken);
            }

            if (r.NextToken == "as")
            {
                c.ColumnName = r.Token.TrimEndSpace();
                return read(r.NextToken);
            }

            if (c.ColumnName == String.Empty)
            {
                var sp = r.Token.TrimEndSpace().Split(Parser.SpaceTokens);
                c.ColumnName = sp[0];
                c.AliasName = sp[sp.Length - 1];
            }
            else
            {
                c.AliasName = r.Token.TrimEndSpace();
            }
            s.Columns.Add(c);

            if (r.NextToken != ",") return false;

            c = new SelectColumnTokenSet();
            return read(null);
        };

        while (fn()) { };

        return r;
    }
}
