using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormshrikeTODO.Model;
                                                                                                    
namespace StormshrikeTODO.Model
{
    /// <summary>
    /// Holds the list of Defined GTD Contexts.
    /// </summary>
    [Serializable]
    public class DefinedContexts
    {
        private Dictionary<string, Context> _idKey = new Dictionary<string, Context>();
        private Dictionary<string, Context> _descrKey = new Dictionary<string, Context>();

        /// <summary>
        /// Returns the number of Defined Contexts.
        /// </summary>
        public int Count
        {
            get
            {
                return _idKey.Count;
            }
        }

        public void Add(Context context)
        {
            _idKey.Add(context.ID, context);
            _descrKey.Add(context.ToString(), context);
        }

        public Context FindIdByDescr(string descr)
        {
            if (_descrKey.ContainsKey(descr))
            {
                return _descrKey[descr];
            }
            else
            {
                return null;
            }
        }

        public Context FindIdByID(string id)
        {
            if (_idKey.ContainsKey(id))
            {
                return _idKey[id];
            }
            else
            {
                return null;
            }
        }

        public void Remove(string id)
        {
            if (_idKey.ContainsKey(id))
            {
                string descr = FindIdByID(id).ToString();
                _idKey.Remove(id);
                _descrKey.Remove(descr);
            }
        }

        public List<Context> GetList()
        {
            return _idKey.Values.ToList<Context>();
        }

        public string GetDescription(string id)
        {
            var ctx = FindIdByID(id);
            return ctx?.ToString() != null ? ctx.ToString() : "<No Description Available>";
        }

        public bool ContainsID(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return false;
            }
            return _idKey.ContainsKey(id);
        }

        public static bool IdentifyDifferences(DefinedContexts oldDC, DefinedContexts newDC,
            out List<Context> newList, out List<Context> chgList, out List<Context> delList)
        {
            newList = new List<Context>();
            chgList = new List<Context>();
            delList = new List<Context>();
            foreach (var ctx1 in oldDC.GetList())
            {
                var ctx2 = newDC.FindIdByID(ctx1.ID);
                if (ctx2 == null)
                {
                    delList.Add(ctx1);
                }
                else
                {
                    if (ctx1.Description != ctx2.Description)
                    {
                        chgList.Add(ctx2);
                    }
                }

            }

            foreach (var ctx2 in newDC.GetList())
            {
                var ctx1 = oldDC.FindIdByID(ctx2.ID);
                if (ctx1 == null)
                {
                    newList.Add(ctx2);
                }
            }

            return !(newList.Count == 0 && chgList.Count == 0 && delList.Count == 0);
        }

        public static bool AreDifferences(DefinedContexts dc1, DefinedContexts dc2)
        {
            return DefinedContexts.IdentifyDifferences(dc1, dc2, out List<Context> newList,
                out List<Context> chgList, out List<Context> delList);
        }
    }
}
