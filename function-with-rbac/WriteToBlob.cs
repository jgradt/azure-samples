using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace function_with_rbac
{
    public static class WriteToBlob
    {
        [FunctionName("WriteToBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string name = req.Query["name"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                if (name != null)
                {
                    var blobWriter = new BlobWriter(log);
                    await blobWriter.WriteToBlobAsync($"Hello, {name}");
                    return (ActionResult)new OkObjectResult($"Hello, {name}");
                }

                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }
            catch(Exception ex)
            {
                log.LogError(ex, "Error");
                throw ex;
            }

        }
    }
}
