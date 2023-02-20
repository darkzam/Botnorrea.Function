using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Botnorrea.Functions
{
    public static class NotificationFunction
    {
        private static HttpClient Client = new HttpClient();

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

                if (req.Headers["X-GitHub-Event"].ToString() != "pull_request")
                {
                    return new OkResult();
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic objectBody = JObject.Parse(requestBody);

                var pullRequestMessageObject = new
                {
                    Action = objectBody?.action,
                    Url = objectBody?.pull_request?.html_url,
                    Title = objectBody?.pull_request?.title,
                    Merged = objectBody?.pull_request?.merged,
                    User = objectBody?.pull_request?.user?.login
                };

                var pullRequestMessageStr = JsonConvert.SerializeObject(pullRequestMessageObject);

                var json = JsonConvert.SerializeObject(new { message = pullRequestMessageStr });
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
