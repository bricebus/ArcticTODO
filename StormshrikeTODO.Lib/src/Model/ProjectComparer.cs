using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StormshrikeTODO.Model
{
    public class ProjectComparer
    {

        public static bool AreListsEquivalent(Collection<Project> prjList1, Collection<Project> prjList2,
            out List<Project> chgList, out List<Project> addList, out List<Project> delList, out List<Project> chgTaskList)
        {
            chgList = new List<Project>();
            addList = new List<Project>();
            delList = new List<Project>();
            chgTaskList = new List<Project>();

            if (prjList1 != null)
            {
                foreach (var prj1 in prjList1)
                {
                    var prj2 = prjList2?.Where(p => p.UniqueID == prj1.UniqueID).FirstOrDefault();
                    if (prj2 == null)
                    {
                        delList.Add(prj1);
                    }

                    if (prj2 != null && !prj1.AreProjectAttributesEquivalentTo(prj2))
                    {
                        chgList.Add(prj2);
                    }

                    if (prj2 != null && !prj1.IsTaskListEquivalentTo(prj2.TaskList))
                    {
                        chgTaskList.Add(prj2);
                    }
                }
            }

            if (prjList2 == null)
            {
                return false;
            }

            foreach (var prj2 in prjList2)
            {
                var prj1 = prjList1?.Where(p => p.UniqueID == prj2.UniqueID).FirstOrDefault();
                if (prj1 == null)
                {
                    addList.Add(prj2);
                }
            }
            return chgList.Count == 0 && addList.Count == 0 && delList.Count == 0 && chgTaskList.Count == 0;
        }
    }
}