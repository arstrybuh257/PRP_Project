using GainBargain.DAL.Entities;
using GainBargain.Parser.Parsers;
using GainBargain.Parser.WebAccess;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GainBargain.Parser
{
    class Program
    {
        static void Main(string[] args)
        {
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

            TestWebPageParsing(input);

            input = new ParserSource
            {
                CategoryId = 0,
                Id = 0,
                MarketId = 0,
                Url = "https://rost.kh.ua/catalog/produktovaya_gruppa-xlebobulochnye_izdeliya-xleb/",
                SelName = "body > div.all.wrapper > div > div.matrix-main > div.tovars-main > div.tovars > div > div > div.item-body > h4 > a",
                SelPrice = "body > div.all.wrapper > div > div.matrix-main > div.tovars-main > div.tovars > div > div > div.item-body > div.item-price > span.price",
                SelImageUrl = "body > div.all.wrapper > div > div.matrix-main > div.tovars-main > div.tovars > div > div > div.item-img > a > img",
                ParserId = 0
            };

            TestWebPageParsing(input);
        }

        private static void TestWebPageParsing(ParserSource input)
        {
            List<string> parsedProducts = new List<string>();
            Console.WriteLine($"Started testing");

            var downloader = new HttpDownloader(input.Url, null, null);
            var page = downloader.GetPage();

            // Create price parser
            var parser = new PageParser(page);

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

            Console.ReadLine();
        }
    }
}

