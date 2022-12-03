﻿using SqModel.Analysis.Builder;
using SqModel.Analysis.Extensions;
using SqModel.Core;
using SqModel.Core.Clauses;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SqModel.Analysis;

public class SelectQueryParser : TokenReader
{
    public SelectQueryParser(string text) : base(text)
    {
    }



    //public static List<ValueBase> ParseValues(string text)
    //{
    //    using var p = new SelectQueryParser(text);
    //    return p.ParseValues().ToList();

    //}
    public void Parse()
    {
        var token = ReadToken();
        //if (!(token.AreEqual("select") || token.AreEqual("with"))) throw new SyntaxException($"near {token} (expect: select or with)");

        if (token.AreEqual("select"))
        {

        }
    }

    public SelectableItem ParseSelectItem()
    {
        var aliastoken = "as";
        var breaktokens = new string[] { ",", "from", "where", "group by", "having", "order by", "union" };
        var operatorTokens = new string[] { "+", "-", "*", "/", "=", "!=", "<>", ">=", "<=", "||" };

        var item = ReadToken();
        IValue? value = null;
        var name = string.Empty;

        //table.column
        if (PeekToken().AreEqual("."))
        {
            var table = item;
            ReadToken();
            name = ReadToken();
            value = new ColumnValue(table, name);
        }

        if (value == null) throw new Exception();

        if (PeekToken().AreContains(breaktokens))
        {
            return new SelectableItem(value, name);
        }

        if (PeekToken().AreContains(operatorTokens))
        {

        }

        if (PeekToken().AreEqual(aliastoken))
        {
            ReadToken();
            name = ReadToken();
        }

        return new SelectableItem(value, name);
    }

    public SelectableItem ParseSelectableItem()
    {
        var breaktokens = new string?[] { null, ",", "from", "where", "group by", "having", "order by", "union" };

        var v = ValueBuilder.Build(this);

        if (PeekToken().AreContains(breaktokens))
        {
            return new SelectableItem(v, v.GetDefaultName());
        }

        if (PeekToken().AreEqual("as"))
        {
            ReadToken();
            if (PeekToken().AreContains(breaktokens))
            {
                throw new SyntaxException($"alias name is not found.");
            }
        }

        return new SelectableItem(v, ReadToken());
    }

    //public IEnumerable<ValueBase> ParseValues()
    //{
    //    do
    //    {
    //        if (PeekToken().AreEqual(",")) ReadToken();
    //        yield return ParseValue();
    //    }
    //    while (PeekToken().AreEqual(","));
    //}


    //private FunctionValue ParseWindowFuncion(string text)
    //{
    //    var dic = new Dictionary<string, ValueCollection>();

    //    using var p = new SelectQueryParser(text);
    //    do
    //    {
    //        var token = p.ReadToken();
    //        var vs = new ValueCollection(p.ParseValues().ToList());
    //        dic.Add(token, vs);

    //    } while (p.PeekOrDefault() != null);

    //    var arg = new WindowFunction();
    //    if (dic.ContainsKey("partition by")) arg.PartitionBy = dic["partition by"];
    //    if (dic.ContainsKey("order by")) arg.OrderBy = dic["order by"];

    //    return new FunctionValue("over", arg);
    //}
}
