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
    }
}
