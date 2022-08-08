using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SelectQueryParser
{
    public ReadTokenResult ParseCondition(SelectTokenSet s)
    {
        ReadWhileSpace();

        var t = new SelectTableTokenSet();
        var tokens = new[] { ".", "(", "=", "<", ">", "!" }.ToList();

        // TODO

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
                t.Schema = r.Token.TrimEndSpace();
                return read(r.NextToken);
            }

            if (r.NextToken == "as")
            {
                t.TableName = r.Token.TrimEndSpace();
                return read(r.NextToken);
            }

            if (t.TableName == String.Empty)
            {
                var sp = r.Token.TrimEndSpace().Split(Parser.SpaceTokens);
                t.TableName = sp[0];
                t.AliasName = sp[sp.Length - 1];
            }
            else
            {
                t.AliasName = r.Token.TrimEndSpace();
            }

            s.Table = t;
            return false;
        };

        while (fn()) { };

        return r;
    }
}
