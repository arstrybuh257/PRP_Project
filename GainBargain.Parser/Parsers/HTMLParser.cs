using System;
using System.Collections.Generic;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;
using GainBargain.Parser.Interfaces;

namespace GainBargain.Parser.Parsers
{

    /// <summary>
    /// Class designed to get sales data from Web pages
    /// via parsing their HTML content
    /// </summary>
    public class HTMLParser : ProductParser
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
        private static readonly Regex priceRegex = new Regex(PRICE_REG_EX);

        /// <summary>
        /// Regular expression for clearing a text up
        /// </summary>
        private static readonly Regex textRegex = new Regex(TEXT_REG_EX);

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

        #region PRODUCTS_PARSING

        /// <summary>
        /// Retrieves all the product's values from page using 
        /// CSS-like selector. If content of the selected
        /// element can't be parsed, the element will be skipped.
        /// </summary>
        /// <typeparam name="T">Product output class to be created.</typeparam>
        /// <param name="input">Objects that stores all the information needed
        /// to parse product objects.</param>
        /// <returns>All the parsed objects.</returns>
        public override IEnumerable<IParserOutput<float>> ParseInformation<T>(IParserInput<float> input)
        {
            PropertyInfo[] propertiesToParse = GetParseableProperties().ToArray();
            // Get amount of parsing properties that must be parsed from web-page
            int selectorPropertiesCount = propertiesToParse.Length;

            // If there are some of them
            if (selectorPropertiesCount > 0)
            {
                // Get html elements corresponding particular properties
                // [X] axis => index of the property from productPropertiesToParse
                // [Y] axis => index of parsed element (entry).
                HtmlNode[][] selectedValues = ParseInputValues(input);

                // Check for the same amount of returned objects
                CheckParsedValuesForEqualCount(selectedValues);

                // Convert Html elements into float, strings etc
                object[,] parsedValues = ProcessParsedElements(selectedValues, input);

                // Enumerate all the objects
                for (int i = 0; i < selectedValues[0].Length; ++i)
                {
                    // Create new product
                    var product = CreateGenericProduct<T>(input);

                    // Set its parsed properties
                    for (int j = 0; j < selectorPropertiesCount; ++j)
                    {
                        propertiesToParse[j].SetValue(product, parsedValues[j, i]);
                    }

                    yield return product;
                }
            }
        }

        /// <summary>
        /// Returns nodes for each input selector.
        /// </summary>
        private HtmlNode[][] ParseInputValues(IParserInput<float> input)
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
        private static object[,] ProcessParsedElements(HtmlNode[][] selectedValues, IParserInput<float> input)
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
                var property = parseableProps[i];

                // How to process each html element
                Func<HtmlNode, object> processor;

                // If this is floating-point value (Price)
                if (property.PropertyType == typeof(float))
                {
                    // Retrieve float value from string
                    processor = el => ParsePrice(el.InnerHtml) ?? 0;
                }
                // If this is an image url
                else if (GetSelectorProperty(property.Name) == nameof(input.SelImageUrl))
                {
                    // Get website domain name
                    var pageHost = new Uri(input.Url).Host;

                    // Image Url is domain name + relative path from src attribute
                    processor = el => pageHost + "/"
                    + el.Attributes
                        .FirstOrDefault(attr => attr.Name == "src")
                        ?.Value
                        ?? throw new Exception("The element does not have src attribute!");
                }
                // If this is text property
                else if (property.PropertyType == typeof(string))
                {
                    // Retrieve string from html
                    processor = el => ParseString(el.InnerHtml);
                }
                // Unknown type
                else
                {
                    throw new Exception($"Unknown type to parse - {property.PropertyType.Name}");
                }

                // Fill processed collection with new values using "processor"
                for (int j = 0; j < selectedValues[i].Length; ++j)
                {
                    processedValues[i, j] = processor(selectedValues[i][j]);
                }
            }

            return processedValues;
        }

        /// <summary>
        /// Checks whether there are the same number of parsed items using
        /// each selector. If it isn't so, throws detailed exception.
        /// </summary>
        /// <param name="selectedValues">An array of parsed values. 
        /// The first dimension - an index of parsing property from productPropertiesToParse;
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


        #endregion PRODUCTS_PARSING

        /// <summary>
        /// Executes CSS query selector and returns found document nodes
        /// </summary>
        /// <param name="querySelector">CSS-like selector. For example,
        /// ".className div #classId"</param>
        protected IEnumerable<HtmlNode> ParseWebPage(string querySelector)
        {
            var document = pageDOM.DocumentNode;
            return document.QuerySelectorAll(querySelector);
        }

        /// <summary>
        /// Parses string in order to get price as float value
        /// </summary>
        /// <param name="elementHtml">InnerHTML of parsed node 
        /// or simply any string that contains number.</param>
        /// <returns>Returns null if the argument does not match
        /// priceRegex regular expression or float valur if it does</returns>
        protected static float? ParsePrice(string elementHtml)
        {
            var priceParts = priceRegex.Matches(elementHtml); // find all the pieces of price
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

            return null;
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

