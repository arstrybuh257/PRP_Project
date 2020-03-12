using SalesParser.Parsers;
using System;
using System.Net;

namespace SalesParser
{
    class Program
    {
        const string URL = "https://www.atbmarket.com/ru/hot/akcii/7day/";
        const string QUERY = @"body .promo_price";

        static void Main(string[] args)
        {
            Console.WriteLine("Downloading site content...");

            // Ask server for a HTML page
            HttpWebRequest oReq = (HttpWebRequest)WebRequest.Create(URL);
            HttpWebResponse resp = (HttpWebResponse)oReq.GetResponse();

            Console.WriteLine("HTML downloaded, parsing...");

            // Create price parser
            var parser = new PageParser(resp.GetResponseStream());
            
            Console.WriteLine($"Executing query: {QUERY}");

            // Go through all the occurences of CSS-like selector
            foreach (var res in parser.ParseWebPage(QUERY))
            {
                // Output the occurence
                Console.WriteLine(res.InnerHtml);

                // Try to parse the price from it
                var price = PageParser.ParsePrice(res.InnerHtml);
                if (price != null)
                {
                    Console.WriteLine($"Read price: {price.Value:f2}");
                }

                Console.WriteLine();
            }

            Console.WriteLine("App has finished its execution");
            Console.Read();
        }
    }
}

