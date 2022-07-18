using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public ReadTokenResult ReadUntilToken()
    {
        var alltokens = CommandTokens.Union(SymbolTokens.Select(x => x.ToString())).Union(SpaceTokens.Select(x => x.ToString())).Union(new[] { LineCommentToken, BlockCommentToken });
        return ReadUntilToken(alltokens);
    }

    public ReadTokenResult ReadUntilToken(IEnumerable<string> tokens)
    {
        Logger?.Invoke(">start ReadUntilToken");

        var cache = new StringBuilder();
        var text = new StringBuilder();
        var result = new ReadTokenResult();

        var finish = (bool isFind) =>
        {
            if (isFind)
            {
                result.NextToken = cache.ToString().ToLower();
                result.Value = text.ToString();
            }
            else
            {
                text.Append(cache);
                result.Value = text.ToString();
            }
            cache.Clear();
            text.Clear();
            return false;
        };

        var startOver = () =>
        {
            text.Append(cache);
            cache.Clear();
            return true;
        };

        var mopUp = () =>
        {
            cache.Append(ReadUntilSpaceOrSymbol());
            cache.Append(ReadWhileSpace());   
            return startOver();
        };

        var isPeekChar = (char c) =>
        {
            var nextc = PeekOrDefault();
            return (nextc != null && nextc.Value == c);
        };

        var fn = () =>
        {
            var cn = PeekOrDefault();
            if (cn == null) return finish(false);

            cache.Append(Read());
            Logger!.Invoke($"cache:{cache}");


            if (cache.IsToken(tokens))
            {
                //Check symbol token
                if (cache.Length == 1)
                {
                    //Check comment symbol token
                    if ((cn.Value == '-' && isPeekChar('-')) || (cn.Value == '/' && isPeekChar('*')))
                    {
                        cache.Append(Read());
                        return finish(true);
                    }
                    return finish(true);
                }

                //Check word token 
                var p = PeekOrDefault();
                if (p == null || p.Value.IsSpace() || p.Value.IsSymbol())
                {
                    return finish(true);
                }
                return mopUp();
            }

            if (cache.IsMaybeToken(tokens)) return true;

            if (cn.IsSpace()) return startOver();

            //Since it was confirmed that it was not a token, proceed until a space appears.
            return mopUp();
        };

        while (fn()) { }

        Logger!.Invoke($"result Text:{result.Value}, Token:{result.NextToken}");
        Logger?.Invoke(">end   ReadUntilToken");

        return result;
    }
}