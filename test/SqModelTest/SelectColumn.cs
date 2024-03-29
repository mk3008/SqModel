﻿using SqModel;
using SqModel.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SqModelTest;

public class SelectColumn
{
    public SelectColumn(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    [Fact]
    public void Default()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a").As("a");

        q.Select.Add().All();
        q.Select.Add().All(ta);
        q.Select.Add().All("a");

        //brief
        q.SelectAll();
        q.SelectAll(ta);
        q.SelectAll("a");

        q.Select.Add().Column(ta, "table_a_id").As("id");
        q.Select.Add().Column("a", "table_a_id").As("id");

        q.Select.Add().Strings(x =>
        {
            x.Add().Column(ta, "table_a_id");
            x.Add().Column(ta, "table_a_id");
            x.Add().Column(ta, "table_a_id");
        }).As("id");

        q.Select.Add().Concat(x =>
        {
            x.Add().Column(ta, "table_a_id");
            x.Add().Column(ta, "table_a_id");
            x.Add().Column(ta, "table_a_id");
        }).As("id");

        //brief
        q.Select(ta, "table_a_id").As("id");
        q.Select("a", "table_a_id").As("id");

        q.Select.Add().Value("a.table_a_id").As("id");
        q.Select.Add().Value(10).As("id");
        q.Select.Add().Value(":val1 + :val2").As("id").AddParameter(":val1", 10).AddParameter(":val2", 20);

        //brief
        q.Select(":val1 + :val2").As("id").AddParameter(":val1", 10).AddParameter(":val2", 20);

        q.Select.Add().InlineQuery(x =>
        {
            x.From("table_b").As("b");
            x.Select("b.id");
        }).As("b_id");

        q.Select.Add().CaseWhen(x =>
        {
            x.Add().When(w => w.Value("a").Equal(1)).Then(10);
            x.Add().When(w => w.Column("a", "id").Equal(2)).Then(20);
            x.Add().When(w => w.Column(ta, "id").Equal(3)).Then(30);
        }).As("case_1");

        q.Select.Add().Case("1", x =>
        {
            x.Add().When("a").Then(10);
            x.Add().When("a", "id").Then(20);
            x.Add().When(ta, "id").Then(30);
            x.Add().When(1).Then(30);
            x.Add().Else(100);
        }).As("case_2");

        var acutal = q.ToQuery();
        //        var expect = @"select table_a.*
        //from table_a
        //where
        //    table_a.id = :id";

        //        Assert.Equal(expect, acutal.CommandText);
        //        Assert.Single(acutal.Parameters);
        //        Assert.Equal(1, acutal.Parameters[":id"]);

        q.GetSelectItems().ForEach(x => Output.WriteLine(x.Name));
    }

    [Fact]
    public void AddRange()
    {
        var q = new SelectQuery();
        var ta = q.From("table_a").As("ta");
        var tb = ta.InnerJoin("table_b").As("tb").On("id");
        q.Select.AddRangeOrDefault(ta, new List<string>() { "a", "b", "c" });
        q.Select.AddRangeOrDefault(tb, new List<string>() { "a", "c", "d" });
        var text = q.ToQuery().CommandText;

        var expect = @"select
    ta.a
    , ta.b
    , ta.c
    , tb.d
from table_a as ta
inner join table_b as tb on ta.id = tb.id";

        Assert.Equal(expect, text);

    }
}
