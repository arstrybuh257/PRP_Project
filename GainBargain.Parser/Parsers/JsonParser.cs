using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GainBargain.Parser.Parsers
{
    /// <summary>
    /// Class for parsing class information from Json string.
    /// </summary>
    public class JsonParser<Input, Output> : ClassParser<Input, Output>
        where Input : new()
        where Output : new()
    {
        /// <summary>
        /// Hierarchy of Json document
        /// </summary>
        protected JObject json;

        /// <summary>
        /// Creates new Json parser of classs. 
        /// </summary>
        /// <param name="json">Json document.</param>
        public JsonParser(string json)
        {
            this.json = JObject.Parse(json);
        }

        /// <summary>
        /// Retrieves all the class' values from Json using 
        /// JsonPath query selector. 
        /// </summary>
        /// <typeparam name="V">Output class to be created.</typeparam>
        /// <param name="input">Objects that stores all the information needed
        /// to parse output objects.</param>        
        /// /// <returns>All the parsed objects.</returns>
        protected override IEnumerable<Output> ParseInformation(Input input)
        {
            // Get all the properties to parse using JsonPath query
            PropertyInfo[] propertiesToParse = GetParseableProperties().ToArray();

            // Check for getting correct parseable properties
            if (propertiesToParse.Length == 0)
            {
                throw new Exception(
                    "Can't parse information 'cause there are no selector properties!");
            }

            // Where to store json matches
            string[][] rawValues = new string[propertiesToParse.Length][];

            // Go though all the parseable properties
            int i = -1;
            foreach(var property in propertiesToParse)
            {
                // Get Json selector value 
                string selectorPropName = GetSelectorProperty(property.Name);
                PropertyInfo selectorProp = input.GetType().GetProperty(selectorPropName);
                string selector = selectorProp.GetValue(input) as string;

                // Find all the matches
                string[] values = json.SelectTokens(selector)
                    .Select(j => j.ToString())
                    .ToArray();

                // Save them
                rawValues[++i] = values;
            }

            // Count of objects parsed
            int objectsParsed = rawValues.First().Length;

            // Check for founding the same amount of values for each parseable property
            if (!rawValues.All(vals => vals.Length == objectsParsed))
            {
                // Display detailed error
                StringBuilder sb = new StringBuilder(
                    "Parsed unequal amount of values!\n");

                for(int j = 0; j < propertiesToParse.Length; ++j)
                {
                    sb.Append($"{propertiesToParse[j]}: {rawValues[j].Length}\n");
                }

                throw new Exception(sb.ToString());
            }

            // Enumerate all the objects
            for (int objIndx = 0; objIndx < objectsParsed; ++objIndx)
            {
                // Create an empty object
                var obj = CreateGenericOutput(input);

                // Set their parsed properties
                for (i = 0; i < rawValues.Length; ++i)
                {
                    // Converted to the appropriate type

                    PropertyInfo prop = propertiesToParse[i];
                    object parsedValue = rawValues[i][objIndx];

                    object convertedValue = Convert.ChangeType(parsedValue, prop.PropertyType);

                    prop.SetValue(obj, convertedValue);
                }

                yield return obj;
            }
        }
    }
}
