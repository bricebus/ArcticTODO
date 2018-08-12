using System;

namespace StormshrikeTODO.Model
{
    /// <summary>
    /// Instances of this class represent a GTD "Context".
    /// </summary>
    [Serializable]
    public class Context
    {
        public string ID { get; set; }
        public string Description { get; set; }

        public Context()
        {

        }

        public Context(string contextID, string contextDescr)
        {
            if (string.IsNullOrEmpty(contextDescr))
            {
                throw new System.ArgumentException("Context Descrpiion cannot be null or empty");
            }

            if (string.IsNullOrEmpty(contextID))
            {
                throw new System.ArgumentException("Context ID cannot be null or empty");
            }

            this.Description = contextDescr;
            ID = contextID;
        }

        public override string ToString()
        {
            return this.Description;
        }

    }
}