using System;
using System.Linq;
using System.Collections.Generic;

namespace StormshrikeTODO.Model
{
    public class TaskComparer
    {
        public TaskComparer()
        {
        }

        public bool AreListsEquivalent(List<Task> taskList1, List<Task> taskList2,
            out List<Task> chgList, out List<Task> addList, out List<Task> delList)
        {
            bool areEquivalent = true;

            chgList = new List<Task>();
            addList = new List<Task>();
            delList = new List<Task>();

            if (taskList1 == null && taskList2 == null)
            {
                return true;
            }

            if (taskList1 != null)
            {
                foreach (var t1 in taskList1)
                {
                    var t2 = taskList2?.Where(t => t.UniqueID == t1.UniqueID).FirstOrDefault();
                    if (t2 == null)
                    {
                        delList.Add(t1);
                        areEquivalent = false;
                    }
                    else if (!t1.IsEquivalentTo(t2))
                    {
                        chgList.Add(t2);
                        areEquivalent = false;
                    }
                }
            }
            
            if (taskList2 == null)
            {
                return false;
            }

            foreach (var t2 in taskList2)
            {
                var t1 = taskList1?.Where(t => t.UniqueID == t2.UniqueID).FirstOrDefault();
                if (t1 == null)
                {
                    addList.Add(t2);
                    areEquivalent = false;
                }
            }

            return areEquivalent;
        }
    }
}