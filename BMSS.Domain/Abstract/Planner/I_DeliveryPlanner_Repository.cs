using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Abstract.Planner
{
    public interface I_DeliveryPlanner_Repository
    {
        bool AddDOPlannerLine(DeliveryPlanner DOPlannerObj, ref string ValidationMessage);
      

    }
}
