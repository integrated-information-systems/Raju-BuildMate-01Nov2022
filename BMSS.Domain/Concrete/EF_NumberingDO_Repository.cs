﻿using BMSS.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BMSS.Domain.Concrete
{
    public class EF_NumberingDO_Repository : I_Numbering_Repository
    {
        public IEnumerable<object> NumberingList
        {
            get
            {
                using (var dbcontext = new DomainDb())
                {
                    return dbcontext.NumberingDO.AsNoTracking().ToList();
                }
            }
        }
        public void SaveNumberings(object ParamNumberingObj)
        {

            using (var dbcontext = new DomainDb())
            {
                NumberingDO NumberingObj = (NumberingDO)ParamNumberingObj;
                if (NumberingObj != null)
                {
                    using (DbContextTransaction transaction = dbcontext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                    {
                        if (NumberingObj.NumberingID == 0)
                        {
                            if (NumberingObj.IsDefault.Equals(true))
                            {
                                // It will clear any defaults 
                                dbcontext.Database.ExecuteSqlCommand(@"update NumberingDO set IsDefault=@IsDefault", new SqlParameter("@IsDefault", false));
                            }
                            int DocEntry = dbcontext.NumberingDO.Count() + 1;
                            NumberingObj.NumberingID = DocEntry;
                            NumberingObj.NextNo = NumberingObj.FirstNo;
                            NumberingObj.CreatedOn = DateTime.Now;
                            NumberingObj.UpdatedOn = null;
                            dbcontext.NumberingDO.Add(NumberingObj);
                            dbcontext.SaveChanges();
                            transaction.Commit();
                        }
                        else
                        {
                            if (NumberingObj.IsDefault.Equals(true))
                            {
                                // It will clear any defaults 
                                dbcontext.Database.ExecuteSqlCommand(@"update NumberingDO set IsDefault=@IsDefault", new SqlParameter("@IsDefault", false));
                            }
                            NumberingDO dbEntry = dbcontext.NumberingDO.Find(NumberingObj.NumberingID);
                            if (dbEntry != null)
                            {
                                dbEntry.SeriesName = NumberingObj.SeriesName;
                                dbEntry.Prefix = NumberingObj.Prefix;
                                dbEntry.FirstNo = NumberingObj.FirstNo;
                                dbEntry.LastNo = NumberingObj.LastNo;
                                dbEntry.IsDefault = NumberingObj.IsDefault;
                                dbEntry.IsLocked = NumberingObj.IsLocked;

                                dbEntry.UpdatedBy = NumberingObj.UpdatedBy;
                                dbEntry.UpdatedOn = DateTime.Now;

                            }
                            dbcontext.SaveChanges();
                            transaction.Commit();

                        }
                    }
                }
            }

        }
        public bool IsSeriesNameAlreadyExist(string SeriesName, int NumberingID = 0)
        {
            NumberingDO NoteObject = null;
            using (var dbcontext = new DomainDb())
            {
                NoteObject = dbcontext.NumberingDO.AsNoTracking().Where(i => !i.NumberingID.Equals(NumberingID) && i.SeriesName.Equals(SeriesName)).FirstOrDefault();
            }
            return NoteObject == null ? false : true;
        }
        public bool IsSeriesOverlaps(int FirstNo, int LastNo, int NumberingID = 0)
        {
            NumberingDO NoteObject = null;
            using (var dbcontext = new DomainDb())
            {
                NoteObject = dbcontext.NumberingDO.AsNoTracking().Where(i => ((FirstNo >= i.FirstNo && FirstNo <= i.LastNo) || (LastNo >= i.FirstNo && LastNo <= i.LastNo)) && !i.NumberingID.Equals(NumberingID)).FirstOrDefault();
            }
            return NoteObject == null ? false : true;
        }
        public bool IsDefault(int NumberingID)
        {
            NumberingDO NoteObject = null;
            using (var dbcontext = new DomainDb())
            {
                NoteObject = dbcontext.NumberingDO.AsNoTracking().Where(i => i.NumberingID.Equals(NumberingID)).FirstOrDefault();
            }
            return NoteObject.IsDefault;
        }
        public object GetNumbering(int NumberingID)
        {
            NumberingDO NoteObject = null;
            using (var dbcontext = new DomainDb())
            {
                NoteObject = dbcontext.NumberingDO.AsNoTracking().Where(i => i.NumberingID.Equals(NumberingID)).FirstOrDefault();
            }
            return NoteObject;
        }
        public void Remove(int numberingID)
        {
            using (var dbcontext = new DomainDb())
            {
                NumberingDO dbEntry = dbcontext.NumberingDO.Find(numberingID);
                if (dbEntry != null)
                {
                    dbcontext.NumberingDO.Remove(dbEntry);
                    dbcontext.SaveChanges();
                }
            }
        }
    }
}