using BMSS.Domain.Abstract.Planner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSS.Domain.Concrete.Planner
{
    public class EF_DeliveryPlanner_Repository : I_DeliveryPlanner_Repository
    {
        public bool AddDOPlannerLine(DeliveryPlanner DOPlannerObj, ref string ValidationMessage)
        {
            bool Result = true;
            try
            {
                using (var dbcontext = new PlannerDb())
                {
                    DateTime FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,0,0,0);
                    DateTime ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,23, 59, 59);

                    DOPlannerObj.SNo = dbcontext.DeliveryPlanners.Where(x => x.CreatedDateTime >= FromDate && x.CreatedDateTime <= ToDate).ToList().Count() + 1;
                    DOPlannerObj.SortSequence = DOPlannerObj.SNo;
                    DOPlannerObj.CreatedDateTime = DateTime.Now;
                    DOPlannerObj.CreatedOn = DateTime.Now;                    
                    dbcontext.DeliveryPlanners.Add(DOPlannerObj);
                    dbcontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                ValidationMessage = e.Message;
                Console.WriteLine(e.Message);                
                Result = false;
            }
            return Result;
        }
    }
}
