using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public interface IRelation : ICondition, IQueryable
{
    string LeftTable { get; set; }

    string RightTable { get; set; }

}