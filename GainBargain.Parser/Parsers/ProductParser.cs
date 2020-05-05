using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GainBargain.Parser.Interfaces;

namespace GainBargain.Parser.Parsers
{
    public abstract class ProductParser : IProductParser<float>
    {
        /// <summary>
        /// What name prefix should be before IParserInput selector properties
        /// </summary>
        private const string SelectorPropertyPrefix = "Sel";

        /// <summary>
        /// What properties of IParserOutput are filled in using selectors
        /// </summary>
        private static PropertyInfo[] productPropertiesToParse;

        /// <summary>
        /// Static constructor of the parser
        /// </summary>
        static ProductParser()
        {
            // Get all the properties that have to be parsed using selectors
            productPropertiesToParse = GetProductPropertiesToParse().ToArray();
        }

        public abstract IEnumerable<IParserOutput<float>> ParseInformation<V>(IParserInput<float> input) 
            where V : IParserOutput<float>, new();


        public static string GetSelectorProperty(string propertyToSelect)
        {
            return SelectorPropertyPrefix + propertyToSelect;
        }

        protected static IEnumerable<PropertyInfo> GetParseableProperties()
        {
            return productPropertiesToParse;
        }

        /// <summary>
        /// Creates product with the given class and input properties.
        /// Note: this method creates an object without filled in parseable
        /// properties, so they must be parsed additionally.
        /// </summary>
        /// <typeparam name="T">Product output class to be created.</typeparam>
        /// <param name="input">Parsing input information.</param>
        protected static T CreateGenericProduct<T>(IParserInput<float> input) 
            where T : IParserOutput<float>, new()
        {
            return new T()
            {
                UploadTime = DateTime.Now,
                MarketId = input.MarketId,
                CategoryId = input.CategoryId,
            };
        }

        /// <summary>
        /// Enumerates all IParserOutput properties that should be parsed using selectors
        /// from IParserInput object.
        /// </summary>
        private static IEnumerable<PropertyInfo> GetProductPropertiesToParse()
        {
            var inputType = typeof(IParserInput<float>);
            var inputProperties = inputType.GetProperties();
            var outputType = typeof(IParserOutput<float>);
            var outputProperties = outputType.GetProperties();

            // Go through every parser output property
            foreach (var outputProperty in outputProperties)
            {
                // Find required property name for input type
                var selectorPropName = SelectorPropertyPrefix + outputProperty.Name;
                // If there is wanted property
                if (inputProperties.Any(p => p.Name == selectorPropName))
                {
                    // Then this is a parsing property
                    yield return outputProperty;
                }
            }
        }
    }
}
