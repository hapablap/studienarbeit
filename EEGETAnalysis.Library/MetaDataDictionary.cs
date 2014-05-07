using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.Library
{
    public class MetaDataDictionary
    {
        Dictionary<string, Dictionary<string, string>> categories;
        Hashtable ht;

        public MetaDataDictionary()
        {
            ht = new Hashtable();
        }

        public void AddKeyValuePair(string category, string key, string value)
        {
            if(!ht.ContainsKey(category))
            {
                ht.Add(category, new Dictionary<string, string>());
            }

            ((Dictionary<string, string>)ht[category]).Add(key, value);
        }

        public Dictionary<string, string> GetCategory(string category)
        {
            if (!ht.ContainsKey(category))
            {
                ht.Add(category, new Dictionary<string, string>());
            }

            return ((Dictionary<string, string>)ht[category]);
        }

        public string GetValue(string key)
        {
            string returnValue = "";

            foreach (Dictionary<string, string> item in ht)
            {
                if(item.TryGetValue(key, out returnValue))
                {
                    break;
                }
            }

            return returnValue;
        }
    }
}
