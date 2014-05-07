using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.Library
{
    public class EEGETData
    {
        public EEGETData(List<List<string>> parsedData)
        {
            long lowestTime = 0;

            foreach (List<string> item in parsedData)
            {
                if(item[0] == "Time")
                {
                    lowestTime = Convert.ToInt64(item[1]);

                    for (int i = 1; i < item.Count; i++)
                    {
                        item[i] = (Convert.ToInt64(item[i]) - lowestTime).ToString();
                    }

                    break;
                }
            }
        }
    }
}
