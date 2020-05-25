using GainBargain.DAL.Entities;
using GainBargain.Parser.Parsers;
using GainBargain.Parser.WebAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GainBargain.WEB.Models
{
    public static class Parser
    {
        public static async Task<IEnumerable<Product>> ParseAsync(ParserSource source)
        {
            // Download web request
            string url = source.Url;
            string responceBody = await (new HttpDownloader(url, null, null).GetPageAsync());

            // Create an appropriate parser
            IClassParser<ParserInput, Product> parser;
            if (source.ParserId == 0)
            {
                parser = new HTMLParser<ParserInput, Product>(responceBody);
            }
            else
            {
                parser = new JsonParser<ParserInput, Product>(responceBody);
            }

            // Create an input
            ParserInput input = new ParserInput(source, source.Market);

            return parser.Parse(input);
        }
    }
}