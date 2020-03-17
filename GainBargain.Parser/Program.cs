using GainBargain.DAL.Entities;
using GainBargain.Parser.Parsers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GainBargain.Parser
{
    //interface IParserInput<T>
    //    where T : struct,
    //              IComparable,
    //              IComparable<T>,
    //              IConvertible,
    //              IEquatable<T>,
    //              IFormattable
    //{
    //    string URL { set; get; }
    //    byte ParserId { set; get; }
    //    int ShopId { set; get; }
    //    string SelPrice { set; get; }
    //    string SelName { set; get; }
    //    string SelImageURL { set; get; }
    //}

    //interface IParserOutput<T>
    //    where T : struct,
    //              IComparable,
    //              IComparable<T>,
    //              IConvertible,
    //              IEquatable<T>,
    //              IFormattable
    //{
    //    int ShopId { set; get; }
    //    string Name { set; get; }
    //    DateTime UploadTime { set; get; }

    //    T Price { set; get; }
    //    string ImageURL { set; get; }
    //}

    class Program
    {
        //const string URL = "https://www.atbmarket.com/ru/hot/akcii/7day/";
        //const string QUERY = @"body .promo_price";

        static void Main(string[] args)
        {
            TestWebPageParsing();
            Console.Read();
        }

        private static void TestWebPageParsing()
        {
            List<string> parsedProducts = new List<string>();
            Console.WriteLine($"Started testing");

            var input = new ParserSource
            {
                CategoryId = 0,
                Id = 0,
                MarketId = 0,
                Url = "https://www.atbmarket.com/ru/hot/akcii/7day/",
                SelName = "li > div.promo_info > span",
                SelPrice = "li > div.promo_info > div.price_box > div.promo_price",
                SelImageUrl = ".container .promo_image_wrap img",
                ParserId = 0
            };

            // Ask server for a HTML page
            HttpWebRequest oReq = (HttpWebRequest)WebRequest.Create(input.Url);
            HttpWebResponse resp = (HttpWebResponse)oReq.GetResponse();
            // Create price parser
            var parser = new PageParser(resp.GetResponseStream());



            // Go through all the occurences of CSS-like selector
            foreach (var res in parser.ParseInformation<Product>(input))
            {
                // Output a parsed price
                parsedProducts.Add($"{res.Name} - {res.Price:f2}UAH img URL: {res.ImageUrl}");
            }

            Console.WriteLine($"Retrieved {parsedProducts.Count} values.");
            foreach (var price in parsedProducts)
            {
                Console.WriteLine(price);
            }

        }
    }
}

