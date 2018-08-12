using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormshrikeTODO.Model.Util
{
    public class Utility
    {
        public static String GetDateTimeString(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToShortDateString();
            }
            else
            {
                return "";
            }
        }
    }
}
