using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GainBargain.Parser.Parsers
{
    public abstract class ClassParser<Input, Output> : IClassParser<Input, Output>
        where Input : new()
        where Output : new()
    {
        /// <summary>
        /// What name prefix should be before IParserInput selector properties
        /// </summary>
        private const string SELECTOR_PROPERTY_PREFIX = "Sel";

        /// <summary>
        /// What properties of IParserOutput are filled in using selectors
        /// </summary>
        private static PropertyInfo[] propsToParse;

        /// <summary>
        /// What properties should be copied from input when parsing.
        /// Key - input property.
        /// Value - output property.
        /// </summary>
        private static KeyValuePair<PropertyInfo, PropertyInfo>[] propsToCopy;

        /// <summary>
        /// What properties must be filled with the time of parsing.
        /// </summary>
        private static PropertyInfo[] propsOfTimeStamp;

        /// <summary>
        /// Methods to invoke before returning a parsed object.
        /// </summary>
        private static MethodInfo[] methodsOfPostProcess;

        /// <summary>
        /// Static constructor of the parser
        /// </summary>
        static ClassParser()
        {
            // Get all the properties that have to be parsed using selectors
            propsToParse = GetProductPropertiesToParse().ToArray();

            // Properties that must be just copied
            propsToCopy = GetPropertiesToCopy().ToArray();

            // Properties where to put current time
            propsOfTimeStamp = GetTimeStampProperties().ToArray();

            // Methods to be called before the element is returned
            methodsOfPostProcess = GetPostProcessMethods().ToArray();
        }

        /// <summary>
        /// Basically, takes input, parses given source and post processes
        /// the result before returning it.
        /// </summary>
        public IEnumerable<Output> Parse(Input input)
        {
            foreach (var obj in ParseInformation(input))
            {
                // Apply them
                foreach (var postProcess in methodsOfPostProcess)
                {
                    postProcess.Invoke(null, new object[] { obj });
                }

                // And after that return an object
                yield return obj;
            }
        }

        /// <summary>
        /// Implement this method to create objects using your source.
        /// All the post processing is done into ClassParser.Parse method.
        /// </summary>
        protected abstract IEnumerable<Output> ParseInformation(Input input);

        /// <summary>
        /// Returns selector property name from the parseable property.
        /// </summary>
        public static string GetSelectorProperty(string propertyToSelect)
        {
            return SELECTOR_PROPERTY_PREFIX + propertyToSelect;
        }

        /// <summary>
        /// Returns all the properties to be parsed by the class.
        /// </summary>
        protected static IEnumerable<PropertyInfo> GetParseableProperties()
        {
            return propsToParse;
        }

        /// <summary>
        /// Creates an output with filled in copy-past and time stamp properties.
        /// Note: this method creates an object without filled in parseable
        /// properties, so they must be parsed additionally.
        /// </summary>
        /// <typeparam name="T">Product output class to be created.</typeparam>
        /// <param name="input">Parsing input information.</param>
        protected static Output CreateGenericOutput(Input input)
        {
            Output obj = new Output();

            // Copy values
            foreach (var copyProps in propsToCopy)
            {
                copyProps.Value.SetValue(obj, copyProps.Key.GetValue(input));
            }

            // Set timestamps
            foreach (var timeStampProp in propsOfTimeStamp)
            {
                timeStampProp.SetValue(obj, DateTime.Now);
            }

            return obj;
        }

        /// <summary>
        /// Enumerates all IParserOutput properties that should be parsed using selectors
        /// from IParserInput object.
        /// </summary>
        private static IEnumerable<PropertyInfo> GetProductPropertiesToParse()
        {
            var inputType = typeof(Input);
            var inputProperties = inputType.GetProperties();
            var outputType = typeof(Output);
            var outputProperties = outputType.GetProperties();

            // Go through every parser output property
            foreach (var outputProperty in outputProperties)
            {
                // Find required property name for input type
                var selectorPropName = SELECTOR_PROPERTY_PREFIX + outputProperty.Name;
                // If there is wanted property
                if (inputProperties.Any(p => p.Name == selectorPropName))
                {
                    // Then this is a parsing property
                    yield return outputProperty;
                }
            }
        }

        /// <summary>
        /// What properties should be copied from input when parsing.
        /// Key - input property.
        /// Value - output property.
        /// </summary>
        private static IEnumerable<KeyValuePair<PropertyInfo, PropertyInfo>> GetPropertiesToCopy()
        {
            var inputProps = typeof(Input).GetProperties();
            var outputProps = typeof(Output).GetProperties();
            return inputProps.Where(
                inProp => outputProps
                    .Any(outProp => inProp.Name == outProp.Name
                        && inProp.PropertyType == outProp.PropertyType))
                   .Select(inputProp => new KeyValuePair<PropertyInfo, PropertyInfo>(
                       key: inputProp,
                       value: outputProps.First(outProp => outProp.Name == inputProp.Name)));
        }

        /// <summary>
        /// What properties must be filled with the time of parsing.
        /// </summary>
        private static IEnumerable<PropertyInfo> GetTimeStampProperties()
        {
            var inputProps = typeof(Input).GetProperties();
            var outputProps = typeof(Output).GetProperties();
            return outputProps.Where(prop => prop.PropertyType == typeof(DateTime))
                .Except(GetPropertiesToCopy()
                    .Select(pair => pair.Value))
                .Except(GetParseableProperties());
        }

        /// <summary>
        /// Methods to invoke before returning a parsed object.
        /// </summary>
        private static IEnumerable<MethodInfo> GetPostProcessMethods()
        {
            var outType = typeof(Output);
            return outType.GetMethods(BindingFlags.NonPublic
                | BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy)
                .Where(m => m.ReturnParameter.ParameterType == typeof(void)
                    && m.GetParameters().Count() == 2
                    && m.GetParameters().First().ParameterType == outType
                    && m.GetParameters().Skip(1).First().ParameterType == typeof(Input));
        }
    }
}
