using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var json = JsonConvert.SerializeObject(new { message = requestBody });
                var content = new StringContent(json);

                await Client.PostAsync(config["Botnorrea.Webhook"], content);

                return new OkResult();
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
