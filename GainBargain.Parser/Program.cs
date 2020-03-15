using SalesParser.Parsers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SalesParser
{
    interface IParserInput<T>
        where T : struct,
                  IComparable,
                  IComparable<T>,
                  IConvertible,
                  IEquatable<T>,
                  IFormattable
    {
        string URL { set; get; }
        byte ParserId { set; get; }
        int ShopId { set; get; }
        string SelPrice { set; get; }
        string SelName { set; get; }
        string SelImageURL { set; get; }
    }

    interface IParserOutput<T> 
        where T : struct,
                  IComparable,
                  IComparable<T>,
                  IConvertible,
                  IEquatable<T>,
                  IFormattable
    {
        int ShopId { set; get; }
        string Name { set; get; }
        DateTime UploadTime { set; get; }

        T Price { set; get; }
        string ImageURL { set; get; }
    }

    class Program
    {
        const string URL = "https://www.atbmarket.com/ru/hot/akcii/7day/";
        const string QUERY = @"body .promo_price";

        static void Main(string[] args)
        {
            TestWebPageParsing(URL, QUERY);
            Console.Read();
        }

        private static async void TestWebPageParsing(string url, string query)
        {
            List<float> parsedPrices = new List<float>();
            Console.WriteLine($"Started testing {url}");
            await Task.Run(() =>
            {
                // Ask server for a HTML page
                HttpWebRequest oReq = (HttpWebRequest)WebRequest.Create(URL);
                HttpWebResponse resp = (HttpWebResponse)oReq.GetResponse();
                // Create price parser
                var parser = new PageParser(resp.GetResponseStream());

                // Go through all the occurences of CSS-like selector
                foreach (var res in parser.GetAllPrices(QUERY))
                {
                    // Output a parsed price
                    parsedPrices.Add(res);
                }
            });

            Console.WriteLine($"Retrieved {parsedPrices.Count} values. Enter 1 to display.");
            if (Console.Read() == '1')
            {
                foreach(var price in parsedPrices)
                {
                    Console.WriteLine(price);
                }
            }
        }
    }
}

