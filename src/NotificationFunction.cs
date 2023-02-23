using Botnorrea.Functions.Strategies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Botnorrea.Functions
{
    public static class NotificationFunction
    {
        private static HttpClient Client = new HttpClient();
        private static Dictionary<string, GetMessageStrategy> RegisteredEvents = new Dictionary<string, GetMessageStrategy>() { { "pull_request", new PullRequestGetMessageStrategy() } };

        [FunctionName("NotificationFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            try
            {
                var config = new ConfigurationBuilder()
                                                 .SetBasePath(context.FunctionAppDirectory)
                                                 .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                                 .AddEnvironmentVariables()
                                                 .Build();

                if (!req.Headers.ContainsKey("X-GitHub-Event"))
                {
                    return new OkResult();
                }

                var eventName = req.Headers["X-GitHub-Event"].ToString();

                if (!RegisteredEvents.ContainsKey(eventName))
                {
                    return new OkResult();
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic payload = JObject.Parse(requestBody);

                if (eventName == "workflow_run"
                    && payload.action.ToString() != "completed")
                {
                    return new OkResult();
                }

                var message = RegisteredEvents[eventName].GetMessage(payload);

                var json = JsonConvert.SerializeObject(new { message = message });
                var content = new StringContent(json);

                await Client.PostAsync(config["Botnorrea.Webhook"], content);

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
