using System;
using System.Collections.Generic;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;

namespace GainBargain.Parser.Parsers
{

    /// <summary>
    /// Class designed to get sales data from Web pages
    /// via parsing their HTML content
    /// </summary>
    public class HTMLParser<Input, Output> : ClassParser<Input, Output>
        where Input : new()
        where Output : new()
    {
        /// <summary>
        /// Regular expression pattern for parsing pieces of prices
        /// </summary>
        private const string PRICE_REG_EX = @"\d+";

        /// <summary>
        /// Regular expression patter for deleting from text
        /// 1) Html tags
        /// 2) Repeated whitespace symbols
        /// 3) Junk symbols(#№)
        /// </summary>
        private const string TEXT_REG_EX = @"(\<.*?\>.*?\<\/.*?\>)|(\s{2,})|[*#№]";

        /// <summary>
        /// Regular expression for parsing pieces of prices
        /// </summary>
        private static readonly Regex numberRegex = new Regex(PRICE_REG_EX);

        /// <summary>
        /// Regular expression for clearing a text up
        /// </summary>
        private static readonly Regex textRegex = new Regex(TEXT_REG_EX);

        private static readonly Regex attributeSelectorRegex = new Regex(@"\s+\{(\S+)?\}");

        /// <summary>
        /// Structure of the loaded HTML document
        /// </summary>
        private HtmlDocument pageDOM;

        /// <summary>
        /// Creates new parser using stream with HTML code.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="pageHtmlCode">Stream with HTML code of the
        /// desired page. For example, it can use System.Net.WebResponce stream.</param>
        public HTMLParser(string pageHtmlCode)
        {
            // Save document structure
            pageDOM = new HtmlDocument();

            //var sr = new StringReader(Encoding.UTF8.GetString(ReadFully(new StreamReader(pageHtmlCode, true).str)));
            pageDOM.LoadHtml(pageHtmlCode);
        }

        #region CLASS_PARSING

        /// <summary>
        /// Retrieves all the class' values from page using 
        /// CSS-like selector. If content of the selected
        /// element can't be parsed, the element will be skipped.
        /// </summary>
        /// <typeparam name="T">Class output class to be created.</typeparam>
        /// <param name="input">Objects that stores all the information needed
        /// to parse output objects.</param>
        /// <returns>All the parsed objects.</returns>
        protected override IEnumerable<Output> ParseInformation(Input input)
        {
            PropertyInfo[] propertiesToParse = GetParseableProperties().ToArray();
            // Get amount of parsing properties that must be parsed from web-page
            int selectorPropertiesCount = propertiesToParse.Length;

            // If there are some of them
            if (selectorPropertiesCount == 0)
            {
                throw new Exception(
                    "There are no parseable fields!");
            }

            // Get html elements corresponding particular properties
            // [X] axis => index of the property from parseable properties
            // [Y] axis => index of parsed element (entry).
            HtmlNode[][] selectedValues = ParseInputValues(input);

            // Check for the same amount of returned objects
            CheckParsedValuesForEqualCount(selectedValues);

            // Convert Html elements into float, strings etc
            object[,] parsedValues = ProcessParsedElements(selectedValues, input);

            // Enumerate all the objects
            for (int i = 0; i < selectedValues[0].Length; ++i)
            {
                // Create new Output
                var output = CreateGenericOutput(input);

                // Set its parsed properties
                for (int j = 0; j < selectorPropertiesCount; ++j)
                {
                    propertiesToParse[j].SetValue(output, parsedValues[j, i]);
                }

                yield return output;
            }
        }

        /// <summary>
        /// Returns nodes for each input selector.
        /// </summary>
        private HtmlNode[][] ParseInputValues(Input input)
        {
            // Array of parsed elements of each property
            HtmlNode[][] selectedValues = new HtmlNode[GetParseableProperties().Count()][];
            // Class of input object
            Type inputType = input.GetType();
            // How much properties have been parsed by now
            int propertiesSelected = 0;

            // Go through all the parsing properties of input object
            foreach (var parsedProperty in GetParseableProperties())
            {
                // Retrieve selector value from input object
                var selectorPropertyName = GetSelectorProperty(parsedProperty.Name);
                var selectorProperty = inputType.GetProperty(selectorPropertyName);
                var selectorValue = selectorProperty.GetValue(input) as string;
#if DEBUG
                // If there are no selector or type is incorrect
                if (selectorValue == null)
                {
                    throw new Exception($"Return type of {selectorProperty.Name} is not string!");
                }
#endif
                // Parse html elements by property's selector
                selectedValues[propertiesSelected++] = ParseWebPage(selectorValue).ToArray();
            }

            return selectedValues;
        }


        /// <summary>
        /// Processes html elements (turns them into an appropriate values).
        /// </summary>
        /// <param name="selectedValues">An array of parsed elements of each property.
        /// The first dimension - an index of parsing property from productPropertiesToParse;
        /// the second dimension - how many objects have been parsed for each property.</param>
        private static object[,] ProcessParsedElements(HtmlNode[][] selectedValues, Input input)
        {
            // Get all the properties to be parsed from the html
            PropertyInfo[] parseableProps = GetParseableProperties().ToArray();
            int parsedObjectsCount = selectedValues[0]?.Length ?? 0;
            int selectorPropertiesCount = parseableProps.Length;

            // Allocate an array of processed objects
            object[,] processedValues = new object[selectorPropertiesCount, parsedObjectsCount];
            for (int i = 0; i < selectorPropertiesCount; ++i)
            {
                // Get parsing property
                PropertyInfo property = parseableProps[i];
                string selectorPropName = GetSelectorProperty(property.Name);
                PropertyInfo selectorProp = input.GetType().GetProperty(selectorPropName);

                // If this is an attribute selector
                string attr = null;
                if (selectorProp.PropertyType == typeof(string))
                {
                    string propertyValue = selectorProp.GetValue(input) as string;
                    Match attributeSelector = attributeSelectorRegex.Match(propertyValue);
                    if (attributeSelector.Success)
                    {
                        // State that we need an attribute value
                        attr = attributeSelector.Groups[1].Value;
                    }
                }

                bool isPropertyNumeric = IsNumericType(property.PropertyType);

                for (int j = 0; j < selectedValues[i].Length; ++j)
                {
                    HtmlNode node = selectedValues[i][j];
                    string strVal;
                    if (attr == null)
                    {
                        strVal = node.InnerHtml;
                    }
                    else
                    {
                        strVal = node.Attributes
                            .FirstOrDefault(a => a.Name == attr)?.Value
                            ?? throw new Exception(
                                $"Node {node.InnerHtml} does not contain attribute {attr}!");
                    }

                    // If parsing a number
                    if (isPropertyNumeric)
                    {
                        processedValues[i, j] = ParseNumber(strVal);
                    }
                    // Parsing a string
                    else
                    {
                        processedValues[i, j] = ParseString(strVal);
                    }
                }
            }

            return processedValues;
        }

        /// <summary>
        /// Checks whether the given type is numeric one.
        /// Code shamelessly copied and pasted from StackOverflow.
        /// </summary>
        private static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether there are the same number of parsed items using
        /// each selector. If it isn't so, throws detailed exception.
        /// </summary>
        /// <param name="selectedValues">An array of parsed values. 
        /// The first dimension - an index of parsing property from parseable properties;
        /// the second dimension - how many objects have been parsed for each property.</param>
        /// <returns>Returns amount of parsed objects.</returns>
        private static int CheckParsedValuesForEqualCount(HtmlNode[][] selectedValues)
        {
            // Get properties to be parsed from html
            PropertyInfo[] propertiesToParse = GetParseableProperties().ToArray();
            int parsedObjectsCount = selectedValues[0].Length;
            int selectorPropertiesCount = propertiesToParse.Length;

            // Check whether an array is not jagged (all the dimensions are equal)
            for (int i = 1; i < selectorPropertiesCount; ++i)
            {
                if (selectedValues[i].Length != parsedObjectsCount)
                {
                    // Generate Exception message
                    var sb = new StringBuilder(
                        "Wrong selector or URL! Make sure that all the " +
                        "selectors return the same amount of matches!\n");
                    for (int j = 0; j < selectorPropertiesCount; ++j)
                    {
                        sb.Append(propertiesToParse[j].Name);
                        sb.Append(':');
                        sb.Append(selectedValues[j].Length);
                        sb.AppendLine();
                    }
                    throw new Exception(sb.ToString());
                }
            }

            return parsedObjectsCount;
        }


        #endregion CLASS_PARSING

        /// <summary>
        /// Executes CSS query selector and returns found document nodes
        /// </summary>
        /// <param name="querySelector">CSS-like selector. For example,
        /// ".className div #classId"</param>
        protected IEnumerable<HtmlNode> ParseWebPage(string querySelector)
        {
            var document = pageDOM.DocumentNode;

            // Exclude custom attribute selection (e.g. div > img {src})
            var attributeSel = attributeSelectorRegex.Match(querySelector);
            if (attributeSel.Success)
            {
                querySelector = querySelector.Substring(0, attributeSel.Index);
            }

            return document.QuerySelectorAll(querySelector);
        }

        /// <summary>
        /// Parses string in order to get price as float value
        /// </summary>
        /// <param name="elementHtml">InnerHTML of parsed node 
        /// or simply any string that contains number.</param>
        /// <returns>Returns null if the argument does not match
        /// priceRegex regular expression or float valur if it does</returns>
        protected static float ParseNumber(string elementHtml)
        {
            var priceParts = numberRegex.Matches(elementHtml); // find all the pieces of price
            var pricePartsCount = priceParts.Count;
            if (pricePartsCount == 1 || // Found only integer
                pricePartsCount == 2)   // Found both integer and fraction part
            {
                // Parsing as int 'cause we don't want it to have any kind of comas and dots
                float price = Int32.Parse(s: priceParts[0].Value);

                // If there is also fraction part
                if (pricePartsCount == 2)
                {
                    // Add fractions to the price
                    price += (float)Int32.Parse(s: priceParts[1].Value) / 100;
                }

                return price;
            }

            return 0f;
        }

        /// <summary>
        /// Removes from string any repeated whitespaces, html tags
        /// and different junk symbols using 
        /// </summary>
        protected static string ParseString(string elementHtml)
        {
            var censored = textRegex.Replace(elementHtml, string.Empty);

            return censored;
        }
    }
}

