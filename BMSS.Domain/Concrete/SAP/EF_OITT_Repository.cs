using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Entities;
using BMSS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BMSS.Domain.Concrete.SAP
{
    public class EF_OITT_Repository : I_OITT_Repository
    {
         
        public IEnumerable<OITT> GetAllOITT
        {
            get
            {
                using (var dbcontext = new EFSapDbContext())
                {
                    return dbcontext.ProductTrees.ToList();

                }
            }
        }

    }
}
