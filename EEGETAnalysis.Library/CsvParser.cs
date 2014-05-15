using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.Library
{
    /// <summary>
    /// The CsvParser class parses the CSV exported from the BeGaze software
    /// </summary>
    public class CsvParser
    {
        /// <summary>
        /// File path to the CSV file
        /// </summary>
        protected string csvFilePath;

        /// <summary>
        /// CSV seperator
        /// </summary>
        protected char seperator = '\t';

        /// <summary>
        /// List contains the parsed CSV content
        /// </summary>
        List<List<string>> parsedCsvContent;

        /// <summary>
        /// This class contains the parsed MetaData in categories
        /// </summary>
        MetaDataDictionary metaData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="csvFilePath">Path to the CSV file</param>
        public CsvParser(string csvFilePath)
        {
            this.csvFilePath = csvFilePath;
            parsedCsvContent = new List<List<string>>();
            metaData = new MetaDataDictionary();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="csvFilePath">Path to the CSV file</param>
        /// <param name="csvSeperator">CSV seperator can be changed, default is TAB</param>
        public CsvParser(string csvFilePath, char csvSeperator) : this(csvFilePath)
        {
            this.seperator = csvSeperator;
        }

        /// <summary>
        /// Executes the parsing process
        /// </summary>
        /// <returns></returns>
        public List<List<string>> Parse()
        {
            StreamReader reader = new StreamReader(File.OpenRead(csvFilePath));
            bool firstRow = true; // indicates that we are on the first row of the CSV data (not meta data), it contains the CSV identifieres
            int csvColumnCount = 0;
            int loopCount;
            string tmpMetaData;
            string tmpCurrentMetaDataCategory = "general";

            // Read the CSV file line by line
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                
                // MetaData begins with ## / check if the current line is meta data
                if (line.StartsWith("##"))
                {
                    // Sometimes a line contains only '##' 
                    if (line.Length > 3)
                    {
                        // Cut off the '## '
                        tmpMetaData = line.ToString().Substring(3);

                        // MetaData category is inside square brackets, check if current line is category
                        // if not, it is meta data
                        if (tmpMetaData[0] == '[' && tmpMetaData[tmpMetaData.Length - 1] == ']')
                        {
                            tmpCurrentMetaDataCategory = tmpMetaData.Substring(1, tmpMetaData.Length - 2);
                        }
                        else
                        {
                            var values = tmpMetaData.Split(seperator);

                            // Try / catch to prevent exception in case there is no key / value pair (out of index on array)
                            try
                            {
                                metaData.AddKeyValuePair(tmpCurrentMetaDataCategory, values[0].Substring(0, values[0].Length-1), values[1]);
                            }
                            catch (Exception) { }
                        }
                    }
                }
                else
                {
                    var values = line.Split(seperator);

                    // first row contains the identifiers, it gets a special behaviour
                    if (firstRow)
                    {
                        csvColumnCount = values.Count();
                        for (int i = 0; i < csvColumnCount; i++)
                        {
                            parsedCsvContent.Add(new List<string>());
                            parsedCsvContent[i].Add(values[i]);
                        }

                        firstRow = false;
                    }
                    else
                    {
                        // values will be assigned to the corresponding identifier by index
                        loopCount = csvColumnCount;
                        if(values.Count() < csvColumnCount)
                        {
                            loopCount = values.Count();
                        }

                        for (int i = 0; i < loopCount; i++)
                        {
                            parsedCsvContent[i].Add(values[i]);
                        }
                    }
                }
            }

            return parsedCsvContent;
        }

        public MetaDataDictionary GetMetaDataDictionary()
        {
            return metaData;
        }
    }
}
