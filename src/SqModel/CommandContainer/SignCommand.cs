using SqModel.Building;
using SqModel.Clause;
using SqModel.Command;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.CommandContainer;

//public class SignCommand : ICommandContainer, IQueryable
//{


//    public ICommand? Command { get; set; } = null;

//    //TODO
//    public string Name { get; set ; } = string.Empty;

//    //TODO
//    public string ColumnName { get; set; } = string.Empty;


//    public virtual Query ToQuery()
//    {
//        if (Command == null || Sign.IsEmpty()) throw new InvalidProgramException();
//        var q = Command.ToQuery();
//        return q.InsertToken($"{Sign}");
//    }
//}

public static class RightCommandExtension
{



}