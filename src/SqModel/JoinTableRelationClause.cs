//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SqModel;

//public class JoinTableRelationClause
//{
//    public List<TableRelationClause> TableRelationClauses { get; set; } = new();

//    public Query ToQuery()
//    {
//        return TableRelationClauses.Select(x => x.ToQuery()).ToList().ToQuery("\r\n");
//    }


//}