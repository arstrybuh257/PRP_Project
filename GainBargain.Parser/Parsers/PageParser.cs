using System;
using System.Collections.Generic;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using GainBargain.DAL.Interfaces;
using System.Reflection;
using GainBargain.DAL.Entities;
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
        private const string DEFAULT_PRICE_REG_EX = @"(\d+)([<>\w]*(\d\d))?";

        private const string SELECTOR_PROPERTY_PREFIX = "Sel";

        /// <summary>
        /// Regular expression for parsing pieces of prices
        /// </summary>
        private static readonly Regex priceRegex = new Regex(DEFAULT_PRICE_REG_EX);

        /// <summary>
        /// Structure of the loaded HTML document
        /// </summary>
        private HtmlDocument pageDOM;

        private static PropertyInfo[] productPropertiesToParse;

        static PageParser()
        {
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

        /// <summary>
        /// Retrieves all the product's values from page using 
        /// CSS-like selector. If content of the selected
        /// element can't be parsed, the element will be skipped.
        /// </summary>
        /// <param name="querySelector">Selector to get a price</param>
        /// <returns>Read price value.</returns>
        public IEnumerable<IParserOutput<float>> ParseInformation(IParserInput<float> input)
        {
            int selectorPropertiesCount = productPropertiesToParse.Length;
             
            var inputType = input.GetType();
            var outputType = typeof(Product);
            var uploadDate = DateTime.Now;

            if (selectorPropertiesCount > 0)
            {
                var selectedValues = ParseInputValues(input);

                // Check for the same amount of returned objects
                int parsedObjectsCount = selectedValues[0].Length;
                for (int i = 1; i < selectorPropertiesCount; ++i)
                {
                    if (selectedValues[i].Length != parsedObjectsCount)
                    {
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


                // Process html elements (turn them into an appropriate type)

                object[,] parsedValues = new object[selectorPropertiesCount, parsedObjectsCount];
                for (int i = 0; i < selectorPropertiesCount; ++i)
                {
                    var property = productPropertiesToParse[i];
                    if (property.PropertyType == typeof(float))  // if this is a price
                    {
                        for (int j = 0; j < parsedObjectsCount; ++j)
                        {
                            parsedValues[i, j] = ParsePrice(selectedValues[i][j].InnerHtml);
                        }
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        for (int j = 0; j < parsedObjectsCount; ++j)
                        {
                            parsedValues[i, j] = selectedValues[i][j].InnerText;
                        }
                    }
                    else
                    {
                        throw new Exception($"Unknown type to parse - {property.PropertyType.Name}");
                    }
                }

                for (int i = 0; i < parsedObjectsCount; ++i)
                {
                    var product = new Product()
                    {
                        UploadTime = uploadDate,
                        MarketId = input.MarketId,
                        CategoryId = input.CategoryId,
                    };

                    for (int j = 0; j < selectorPropertiesCount; ++j)
                    {
                        productPropertiesToParse[j].SetValue(product, parsedValues[j, i]);
                    }

                    yield return product;
                }

            }


            //foreach (var priceElement in ParseWebPage(querySelector))
            //{
            //    string priceElementHtml = priceElement.InnerHtml;
            //    float? parsedPrice = ParsePrice(priceElementHtml);
            //    if (parsedPrice.HasValue)
            //    {
            //        yield return parsedPrice.Value;
            //    }
            //}
        }

        /// <summary>
        /// Returns nodes for each input selector
        /// </summary>
        private HtmlNode[][] ParseInputValues(IParserInput<float> input)
        {
            Type inputType = input.GetType();
            HtmlNode[][] selectedValues = new HtmlNode[productPropertiesToParse.Length][];
            int propertiesSelected = 0;
            foreach (var parsedProperty in productPropertiesToParse)
            {
                var selectorPropertyName = SELECTOR_PROPERTY_PREFIX + parsedProperty.Name;
                var selectorProperty = inputType.GetProperty(selectorPropertyName);
                var selectorValue = selectorProperty.GetValue(input) as string;
#if DEBUG
                if (selectorValue == null)
                {
                    throw new Exception($"Return type of {selectorProperty.Name} is not string!");
                }
#endif
                selectedValues[propertiesSelected++] = ParseWebPage(selectorValue).ToArray();
            }

            return selectedValues;
        }

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

        private static IEnumerable<PropertyInfo> GetProductPropertiesToParse()
        {
            var inputType = typeof(IParserInput<float>);
            var inputProperties = inputType.GetProperties();
            var outputType = typeof(IParserOutput<float>);
            var outputProperties = outputType.GetProperties();

            foreach (var outputProperty in outputProperties)
            {
                var selectorPropName = SELECTOR_PROPERTY_PREFIX + outputProperty.Name;
                if (inputProperties.Any(p => p.Name == selectorPropName))
                {
                    yield return outputProperty;
                }
            }
        }
    }
}

