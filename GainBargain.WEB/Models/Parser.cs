using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.Parser.Parsers;
using GainBargain.Parser.WebAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GainBargain.WEB.Models
{
    public class Parser
    {
        public static int MAX_PROCESSING_SOURCES = 5;

        public static ParsingState ParsingProgress = new ParsingState();

        private GainBargainContext db = new GainBargainContext();
        private IParserSourceRepository parserSourceRepository;
        private IDbLogsRepository dbLogsRepository;

        private Parser()
        {
            parserSourceRepository = new ParserSourceRepository(db);
            dbLogsRepository = new DbLogsRepository(db);
        }

        public static void Start()
        {
            if (!ParsingProgress.IsParsing)
            {
                new Parser().InitParsing();
            }
        }

        private void InitParsing()
        {

            // If we're parsing now
            if (ParsingProgress.IsParsing)
            {
                // Somebody wants to start parsing again
                return;
            }

            // Get sources to parse
            var sources = db.ParserSources
                .Where(s => s.CategoryId == 11)
                .Include(s => s.Market)
                .ToList();

            try
            {
                // Tell the system that the parsing had started
                ParsingProgress.ParsingStarted(sources.Count);
                dbLogsRepository.Log(DbLog.LogCode.Info, $"Started parsing of {sources.Count} sources.");

                using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(MAX_PROCESSING_SOURCES))
                {
                    List<Task> parsings = new List<Task>();
                    object parsedIncrLock = new object();

                    foreach (ParserSource source in sources)
                    {
                        concurrencySemaphore.Wait();

                        // If all threads are running
                        parsings.Add(Task.Run(async () =>
                        {
                            try
                            {
                                // Create new context for sending batched products inserts
                                var ctxt = new GainBargainContext();

                                // Create the command for inserting products
                                using (var productInsert = new ProductInsertCommand(ctxt))
                                {
                                    // Insert every parsed product
                                    foreach (Product p in await Models.Parser.ParseAsync(source))
                                    {
                                        productInsert.ExecuteOn(p);
                                    }
                                }

                                // For tracking parsing progress
                                lock (parsedIncrLock)
                                {
                                    // Increment processed parsing sources count
                                    ParsingProgress.IncrementDoneSources();
                                }
                            }
                            catch (Exception ex)
                            {
                                dbLogsRepository.Log(DbLog.LogCode.Error, ex.Message);
                            }
                            finally
                            {
                                // If thread is failed, release semaphore
                                concurrencySemaphore.Release();
                            }
                        }));
                    }

                    // Wait for all the tasks to be completed
                    Task.WaitAll(parsings.ToArray());
                }
            }
            finally
            {

                dbLogsRepository.Log(DbLog.LogCode.Info, "Finished parsing. Starting omptimization.");

                // Set timeout to 60 minutes
                int? defTimeout = db.Database.CommandTimeout;
                db.Database.CommandTimeout = 60 * 60;

                // Remove already existing entries
                db.Database.ExecuteSqlCommand("RemoveDuplicates");

                // Update product's cache
                db.Database.ExecuteSqlCommand("UpdateProductsCache");

                db.Database.CommandTimeout = defTimeout;

                dbLogsRepository.Log(DbLog.LogCode.Info, "Optimization is over. Parsing is done.");

                // In any case parsing must finish here
                ParsingProgress.ParsingFinished();
            }
        }

        private static async Task<IEnumerable<Product>> ParseAsync(ParserSource source)
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