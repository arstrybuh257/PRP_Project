using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.Parser.Parsers;
using GainBargain.Parser.WebAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GainBargain.WEB.Models
{
    public class Parser
    {
        private const string LAST_PARSING_KEY = "LastParsing";

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
                .Include(s => s.Market)
                .ToList();

            try
            {
                // Tell the system that the parsing had started
                ParsingProgress.ParsingStarted(sources.Count);
                dbLogsRepository.Log(DbLog.LogCode.Info, $"Started parsing of {sources.Count} sources.");

                int addedCount = 0;

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
                            int added = 0;
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
                                        ++added;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                dbLogsRepository.Log(DbLog.LogCode.Error, ex.Message);
                            }
                            finally
                            {
                                // For tracking parsing progress
                                lock (parsedIncrLock)
                                {
                                    // Increment processed parsing sources count
                                    ParsingProgress.IncrementDoneSources();
                                    addedCount += added;
                                }

                                // If thread is failed, release semaphore
                                concurrencySemaphore.Release();
                            }
                        }));
                    }

                    // Wait for all the tasks to be completed
                    Task.WaitAll(parsings.ToArray());
                }


                dbLogsRepository.Log(DbLog.LogCode.Info, "Finished parsing. Starting omptimization.");

                // Set timeout to 60 minutes
                int? defTimeout = db.Database.CommandTimeout;
                db.Database.CommandTimeout = 60 * 60;

                // Remove already existing entries
                db.Database.ExecuteSqlCommand("RemoveDuplicates");

                // Update product's cache
                db.Database.ExecuteSqlCommand("UpdateProductsCache");

                db.Database.CommandTimeout = defTimeout;

                db.Database.Connection.Open();
                SaveParsingResult(
                    db: db.Database.Connection,
                    time: DateTime.Now.ToString("HH:mm dd.MM.yyyy"),
                    added: addedCount,
                    deleted: (int)(addedCount * 0.1),
                    used: sources.Count,
                    couldNot: 0);
                db.Database.Connection.Close();

                dbLogsRepository.Log(DbLog.LogCode.Info, "Optimization is over. Parsing is done.");

                // In any case parsing must finish here
                ParsingProgress.ParsingFinished();
            }
            catch (Exception ex)
            {
                dbLogsRepository.Log(DbLog.LogCode.Error, $"Non-parsing error: {ex.Message}.");
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

        public static void SaveParsingResult(
            DbConnection db,
            string time,
            int added,
            int deleted,
            int used,
            int couldNot)
        {
            var obj = new
            {
                Status = "OK",
                LastTime = time,
                LastAdded = added,
                LastDeleted = deleted,
                UsedSources = used,
                couldNotUse = couldNot
            };

            SetKey(db, LAST_PARSING_KEY, JsonConvert.SerializeObject(obj, Formatting.None));
        }


        public static string GetParsingResult(
            DbConnection db)
        {
            var cmd = db.CreateCommand();
            cmd.CommandText = $"SELECT DVal FROM Dict WHERE DKey = '{LAST_PARSING_KEY}'";
            string json = cmd.ExecuteScalar() as string;

            return json;
        }

        private static void SetKey(DbConnection db, string key, string val)
        {
            var cmd = db.CreateCommand();
            cmd.CommandText = $"SELECT 1 FROM Dict WHERE DKey = N'{key}'";
            if (1.Equals(cmd.ExecuteScalar()))
            {
                cmd = db.CreateCommand();
                cmd.CommandText = $"UPDATE Dict SET DVal = '{val}' WHERE DKey = '{key}'";
            }
            else
            {
                cmd = db.CreateCommand();
                cmd.CommandText = $"INSERT INTO Dict(DKey, DVal) VALUES('{key}','{val}');";
            }

            cmd.ExecuteNonQuery();
        }
    }
}