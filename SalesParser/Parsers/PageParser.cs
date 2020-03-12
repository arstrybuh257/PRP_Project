using System;
using System.Collections.Generic;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace SalesParser.Parsers
{
    /// <summary>
    /// Class designed to get sales data from Web pages
    /// via parsing their HTML content
    /// </summary>
    public class PageParser
    {
        /// <summary>
        /// Structure of the loaded HTML document
        /// </summary>
        private HtmlDocument pageDOM;

        /// <summary>
        /// Regular expression pattern for parsing pieces of prices
        /// </summary>
        private const string DEFAULT_PRICE_REG_EX = @"(\d+)([<>\w]*(\d\d))?";

        /// <summary>
        /// Regular expression for parsing pieces of prices
        /// </summary>
        private static readonly Regex priceRegex = new Regex(DEFAULT_PRICE_REG_EX);

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
        /// Executes CSS query selector and returns found document nodes
        /// </summary>
        /// <param name="querySelector">CSS-like selector. For example,
        /// ".className div #classId"</param>
        public IEnumerable<HtmlNode> ParseWebPage(string querySelector)
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
        public static float? ParsePrice(string elementHtml)
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
    }
}

