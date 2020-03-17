﻿using System;
using System.Collections.Generic;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using GainBargain.DAL.Interfaces;
using System.Reflection;
using System.Text;

namespace GainBargain.Parser.Parsers
{

    /// <summary>
    /// Class designed to get sales data from Web pages
    /// via parsing their HTML content
    /// </summary>
    public class PageParser : IProductParser<float>
    {
        /// <summary>
        /// Regular expression pattern for parsing pieces of prices
        /// </summary>
        private const string PRICE_REG_EX = @"(\d+)([<>\w]*(\d\d))?";

        /// <summary>
        /// Regular expression patter for deleting from text
        /// 1) Html tags
        /// 2) Repeated whitespace symbols
        /// 3) Junk symbols(#№)
        /// </summary>
        private const string TEXT_REG_EX = @"(\<.*?\>.*?\<\/.*?\>)|(\s{2,})|[*#№]";

        /// <summary>
        /// What name prefix should be before IParserInput selector propertuies
        /// </summary>
        private const string SELECTOR_PROPERTY_PREFIX = "Sel";

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
        /// What properties of IParserOutput are filled in using selectors
        /// </summary>
        private static PropertyInfo[] productPropertiesToParse;

        /// <summary>
        /// Static constructor of the parser
        /// </summary>
        static PageParser()
        {
            // Get all the properties that have to be parsed using selectors
            productPropertiesToParse = GetProductPropertiesToParse().ToArray();
        }

        /// <summary>
        /// Creates new parser using stream with HTML code.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="pageHtmlCode">Stream with HTML code of the
        /// desired page. For example, it can use System.Net.WebResponce stream.</param>
        public PageParser(Stream pageHtmlCode)
        {
            // Save document structure
            pageDOM = new HtmlDocument();
            pageDOM.Load(pageHtmlCode, false);
        }

        #region PRODUCTS_PARSING

        /// <summary>
        /// Retrieves all the product's values from page using 
        /// CSS-like selector. If content of the selected
        /// element can't be parsed, the element will be skipped.
        /// </summary>
        /// <param name="querySelector">Selector to get a price</param>
        /// <returns>Read price value.</returns>
        public IEnumerable<IParserOutput<float>> ParseInformation<T>(IParserInput<float> input)
            where T : IParserOutput<float>, new()
        {
            // Get amount of parsing properties that must be parsed from web-page
            int selectorPropertiesCount = productPropertiesToParse.Length;

            // If there are some of them
            if (selectorPropertiesCount > 0)
            {
                // Get current upload date
                var uploadDate = DateTime.Now;

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
                    var product = new T()
                    {
                        UploadTime = uploadDate,
                        MarketId = input.MarketId,
                        CategoryId = input.CategoryId,
                    };

                    // Set its parsed properties
                    for (int j = 0; j < selectorPropertiesCount; ++j)
                    {
                        productPropertiesToParse[j].SetValue(product, parsedValues[j, i]);
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
            HtmlNode[][] selectedValues = new HtmlNode[productPropertiesToParse.Length][];
            // Class of input object
            Type inputType = input.GetType();
            // How much properties have been parsed by now
            int propertiesSelected = 0;

            // Go through all the parsing properties of input object
            foreach (var parsedProperty in productPropertiesToParse)
            {
                // Retrieve selector value from input object
                var selectorPropertyName = SELECTOR_PROPERTY_PREFIX + parsedProperty.Name;
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
        /// <returns></returns>
        private static object[,] ProcessParsedElements(HtmlNode[][] selectedValues, IParserInput<float> input)
        {
            int parsedObjectsCount = selectedValues[0]?.Length ?? 0;
            int selectorPropertiesCount = productPropertiesToParse.Length;

            // Allocate an array of processed objects
            object[,] processedValues = new object[selectorPropertiesCount, parsedObjectsCount];
            for (int i = 0; i < selectorPropertiesCount; ++i)
            {
                // Get parsing property
                var property = productPropertiesToParse[i];

                // How to process each html element
                Func<HtmlNode, object> processor;

                // If this is floating-point value (Price)
                if (property.PropertyType == typeof(float))
                {
                    // Retrieve float value from string
                    processor = el => ParsePrice(el.InnerHtml) ?? 0;
                }
                // If this is an image url
                else if (SELECTOR_PROPERTY_PREFIX + property.Name == nameof(input.SelImageUrl))
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
            int parsedObjectsCount = selectedValues[0].Length;
            int selectorPropertiesCount = productPropertiesToParse.Length;

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
                        sb.Append(productPropertiesToParse[j].Name);
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
            var priceSearch = priceRegex.Match(elementHtml); // find all the pieces of price
            var numberParts = priceSearch.Groups;

            if (numberParts.Count == 2          // found only 1) int price
               || numberParts.Count == 4)       // found following constructions:
                                                // 1) int price
                                                // 2) additional part with coins
                                                // 3) pennies
            {
                // Parsing as int 'cause we don't want it to have any kind of comas and dots
                float price = Int32.Parse(
                    s: numberParts[1].Value);

                // If there is also pennies part
                if (numberParts.Count >= 4)
                {
                    // Add pennies to the price
                    price += (float)Int32.Parse(s: numberParts[3].Value) / 100;
                }

                return price;
            }

            return null;
        }

        /// <summary>
        /// Removes from string any repeated whitespaces, html tags
        /// and different junk symbols using 
        /// </summary>
        /// <param name="elementHtml"></param>
        /// <returns></returns>
        protected static string ParseString(string elementHtml)
        {
            var censored = textRegex.Replace(elementHtml, string.Empty);

            return censored;
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
                var selectorPropName = SELECTOR_PROPERTY_PREFIX + outputProperty.Name;
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

