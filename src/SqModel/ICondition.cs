using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public interface ICondition : IQueryable
{
    string Operator { get; set; }

    string SubOperator { get; set; }
}