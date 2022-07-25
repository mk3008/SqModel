using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public TableClause ParseTableClause()
    {
        var t = new TableClause();
        var level = 1;

        foreach (var token in ReadAllTokens().Where(x => !x.StartsWith("--") && !x.StartsWith("/*")))
        {
            if (token == "(")
            {
                level++;
                continue;
            }
            if (token == ")")
            {
                level--;
                if (level == 0) break;
            }
            if (token.ToLower() == "where") break;
            if (token.ToLower() == "as") continue;
            if (t.TableName == String.Empty)
            {
                t.TableName = token;
                continue;
            }
            if (t.AliasName == String.Empty)
            {
                t.AliasName = token;
                continue;
            }
        }

        if (t.AliasName == String.Empty) t.AliasName = t.TableName;
        return t;
    }
}
