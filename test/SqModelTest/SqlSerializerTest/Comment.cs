using SqModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace SqModelTest.SqlSerializerTest;

public class Comment
{
    [Fact]
    public void MultipleComment()
    {
        var text = @"/*comment*/";
        var expect = @"";
        var actual = SqlSerializer.RemoveComment(text);
        Assert.Equal(expect, actual);
    }

    [Fact]
    public void MultipleComment_Nest()
    {
        var text = @"/*/*comment*/";
        var expect = @"";
        var actual = SqlSerializer.RemoveComment(text);
        Assert.Equal(expect, actual);
    }

    [Fact]
    public void MultipleComment_Single()
    {
        var text = @"start/*comment*/end";
        var expect = @"startend";
        var actual = SqlSerializer.RemoveComment(text);
        Assert.Equal(expect, actual);
    }

    [Fact]
    public void MultipleComment_Multiple ()
    {
        var text = @"start
/*
comment
*/
end";
        var expect = @"start

end";
        var actual = SqlSerializer.RemoveComment(text);
        Assert.Equal(expect, actual);
    }

    [Fact]
    public void SingleComment()
    {
        var text = @"--";
        var expect = @"";

        var actual = SqlSerializer.RemoveComment(text);
        Assert.Equal(expect, actual);
    }

    [Fact]
    public void SingleComment_Single()
    {
        var text = @"start--end";
        var expect = @"start";

        var actual = SqlSerializer.RemoveComment(text);
        Assert.Equal(expect, actual);
    }

    [Fact]
    public void SingleComment_NewLine()
    {
        var text = @"start--
end";
        var expect = @"start
end";

        var actual = SqlSerializer.RemoveComment(text);
        Assert.Equal(expect, actual);
    }

    [Fact]
    public void Mix_SingleMultiple()
    {
        var text = @"start--/*comment*/end";
        var expect = @"start";
        var actual = SqlSerializer.RemoveComment(text);
        Assert.Equal(expect, actual);
    }

    [Fact]
    public void Mix_MultipleSingle()
    {
        var text = @"start/*--comment*/end";
        var expect = @"startend";
        var actual = SqlSerializer.RemoveComment(text);
        Assert.Equal(expect, actual);
    }
}