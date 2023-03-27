using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Botnorrea.Functions
{
    public static class DebtorsFunction
    {
        private static HttpClient Client = new HttpClient();

        [FunctionName("DebtorsFunction")]
        public static async Task Run([TimerTrigger("0 0 14 * * *")] TimerInfo myTimer,
                                    ILogger log,
                                    ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(context.FunctionAppDirectory)
                            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();

            try
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"{config["DebtManager.Url"]}/api/reports/debts"),
                    Method = HttpMethod.Get,
                };

                request.Headers.Add("api_key", config["DebtManager.ApiKey"]);

                var result = await Client.SendAsync(request);

                string requestBody = await result.Content.ReadAsStringAsync();
                dynamic payload = JArray.Parse(requestBody);

                var messageBuilder = new StringBuilder();

                foreach (var debt in payload)
                {
                    messageBuilder.AppendLine($"Deuda Pendiente: {debt.debtTitle}");
                    messageBuilder.AppendLine($"GranTotal: {debt.grandTotal}");
                    messageBuilder.AppendLine($"Fecha: {debt.date}");

                    foreach (var debtor in debt.debtors)
                    {
                        messageBuilder.AppendLine($"Usuario: @{debtor.username} Total: {debtor.totalPayment}");

                        var items = new List<string>();

                        foreach (var charge in debtor.charges)
                        {
                            items.Add((string)charge.productName);
                        }

                        var groupByProduct = items.GroupBy(x => x)
                                                  .Select(x => new
                                                  {
                                                      productName = x.Key,
                                                      amount = x.Count()
                                                  });

                        foreach (var item in groupByProduct)
                        {
                            messageBuilder.AppendLine($"\t{item.amount} : {item.productName}");
                        }
                    }
                }

                var json = JsonConvert.SerializeObject(new { message = messageBuilder.ToString() });
                var content = new StringContent(json);

                await Client.PostAsync(config["Botnorrea.Webhook"], content);
            }
            catch
            {
                throw;
            }
        }
    }
}
