using GainBargain.DAL.Entities;
using GainBargain.Parser.Parsers;
using GainBargain.Parser.WebAccess;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GainBargain.DAL.EF;
using static GainBargain.Parser.IO.ConsoleIO;

namespace GainBargain.Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Choose what you want to do");
                Console.WriteLine("0 - Start parsing");
                Console.WriteLine("1 - Test parsing input");
                Console.WriteLine("2 - Edit parsing input");
                Console.WriteLine("Smth different to EXIT");

                int result = AskInt();
                switch (result)
                {
                    case 0:
                        StartParsing();
                        break;
                    case 1:
                        TestParsingInput();
                        break;
                    case 2:
                        EditInput();
                        break;
                    default: return;
                }
            }

            /*

RUN THE PROGRAM AND TYPE IN THE FOLLOWING TO TEST INPUT:
1
https://rost.kh.ua/catalog/produktovaya_gruppa-xlebobulochnye_izdeliya-xleb/
body > div.all.wrapper > div > div.matrix-main > div.tovars-main > div.tovars > div > div > div.item-body > h4 > a
body > div.all.wrapper > div > div.matrix-main > div.tovars-main > div.tovars > div > div > div.item-body > div.item-price > span.price
body > div.all.wrapper > div > div.matrix-main > div.tovars-main > div.tovars > div > div > div.item-img > a > img


WORKED EARLIER AND NOW IT DOES NOT
1
https://www.atbmarket.com/ru/hot/akcii/7day/
li > div.promo_info > span
li > div.promo_info > div.price_box > div.promo_price
.container .promo_image_wrap img

            */
        }

        private static void StartParsing()
        {
            // I NEED DB
            throw new NotImplementedException();
        }

        private static void TestParsingInput()
        {
            // Get all the info for parsing
            var url = AskString("Enter URL of the web-page");
            var selName = AskString("Enter selector for product's name");
            var selPrice = AskString("Enter selector for product's price");
            var selImageUrl = AskString("Enter selector for product's image URL");

            try
            {
                // Actually parsing the page

                var input = new ParserSource
                {
                    Url = url,
                    SelName = selName,
                    SelPrice = selPrice,
                    SelImageUrl = selImageUrl
                };

                TestWebPageParsing(input);
            }
            catch (Exception ex)
            {
                // Display error message
                Console.WriteLine(ex.Message);
            }
        }

        private static void EditInput()
        {
            // I NEED DB
            throw new NotImplementedException();
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

