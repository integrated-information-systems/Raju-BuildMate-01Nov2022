using System;
using System.Collections.Generic;

namespace BMSS.Domain.Abstract
{
    public interface I_Numbering_Repository
    {
        IEnumerable<Object> NumberingList { get; }
        void SaveNumberings(Object numberingSQ);
        bool IsSeriesNameAlreadyExist(string SeriesName, int NumberingID = 0);
        bool IsSeriesOverlaps(int FirstNo,int LastNo, int NumberingID = 0);
        bool IsDefault(int NumberingID);

        Object GetNumbering(int NumberingID);
        void Remove(int numberingID);
    }
}
