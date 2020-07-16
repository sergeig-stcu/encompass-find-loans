using System;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace encompass_find_loans {
    class Program {
        static async Task Main(string[] args) {
            if (args.Length != 1) {
                Console.WriteLine("SYNTAX: config.json", args);
                return;
            }
            CosmosClient client;
            Config conf;
            {
                var configFileName = args[0];
                conf = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFileName));
                string epUrl = string.Format("https://{0}.documents.azure.com", conf.Instance);
                var ops = new CosmosClientOptions() {
                    ConnectionMode = ConnectionMode.Gateway,

                };
                client = new CosmosClient(epUrl, conf.AuthKey, ops);
            }
            var account = await client.ReadAccountAsync();
            var container = client.GetContainer(conf.DbID, conf.ContainerID);

            await FindLoansAsync(container);
        }

        private static async Task FindLoansAsync(Container container) {
            var cleanLoans = await FindLoansWithoutPreviousStateAsync(container);
            var events = new List<Event>();
            foreach (var cleanLoan in cleanLoans) {
                var ev = await ReadEventAsync(container, cleanLoan.LoanNumber);
                Console.WriteLine($"Loaded {cleanLoan.LoanNumber} loan with {ev.Origin.Data.currentState.stcuMilestone} milestone");
                events.Add(ev);
            }

            // events by stcuMilestone
            var eventsByMilestone = events.ToLookup<Event, string, Event>(ev => Convert.ToString(ev.Origin.Data.currentState.stcuMilestone), ev => ev);
            foreach (var milestoneGroup in eventsByMilestone) {
                Console.WriteLine($"Milestone: {milestoneGroup.Key}");
                // new method of selection: sort by milestone current date utc and then take 1st 10
                var selection = milestoneGroup.OrderByDescending(ev => (DateTime)ev.Origin.Data.currentState.milestoneCurrentDateUtc).Take(10);
                // var selection = milestoneGroup.Take(10); // old method of selection - just 1st 10.

                foreach (var ev in selection) {
                    var utc = (DateTime)ev.Origin.Data.currentState.milestoneCurrentDateUtc;
                    var local = utc.ToLocalTime();
                    Console.WriteLine($"Loan: {ev.Origin.Data.currentState.loanNumber}, Milestone: {milestoneGroup.Key}, Date (local): {local}   Date (UTC): {utc}");
                }
                Console.WriteLine();
            }
            return;
        }

        private static async Task<List<SRLoanNumber>> FindLoansWithoutPreviousStateAsync(Container container) {
            var query = @"
select c.origin.data.currentState.loanNumber as LoanNumber from Items c
where c.origin.systemName='Encompass' and is_null(c.origin.data.previousState)
group by c.origin.data.currentState.loanNumber
            ";
            return await ReadRecsAsync<SRLoanNumber>(container, query);
        }



        private static async Task<List<T>> ReadRecsAsync<T>(Container container, String query) {
            var rv = new List<T>();
            var qd = new QueryDefinition(query);
            var iterator = container.GetItemQueryIterator<T>(qd);
            while (iterator.HasMoreResults) {
                var results = await iterator.ReadNextAsync();
                foreach (var e in results) {
                    rv.Add(e);
                }
            }
            return rv;
        }

        private static async Task<Event> ReadEventAsync(Container container, String loanNumber) {
            var query = $"select top 1 * from Items c where c.origin.data.currentState.loanNumber = '{loanNumber}' order by c._ts DESC";
            var items = await ReadRecsAsync<Event>(container, query);
            return items.First();
        }

    } // end of class
}
