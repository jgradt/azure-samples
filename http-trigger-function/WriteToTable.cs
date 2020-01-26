using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

namespace HttpTriggerFunction1
{
    public static class WriteToTable
    {
        [FunctionName("WriteToTable")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            // insert into table storage
            if (!string.IsNullOrEmpty(name))
            {
                var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("storage_conn_string"));

                var messageTable = await GetTableAsync(storageAccount, "Messages", log);
                var message = await InsertOrMergeEntityAsync(messageTable, new Message($"Hello, {name}"), log);
            }

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        public static async Task<Message> InsertOrMergeEntityAsync(CloudTable table, Message entity, ILogger log)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                Message insertedCustomer = result.Result as Message;

                return insertedCustomer;
            }
            catch (StorageException e)
            {
                log.LogError(e, "Error while writing to table storage");
                throw;
            }
        }

        public static async Task<CloudTable> GetTableAsync(CloudStorageAccount storageAccount, string tableName, ILogger log)
        {
            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);
            if (await table.CreateIfNotExistsAsync())
            {
                log.LogInformation("Created Table named: {0}", tableName);
            }

            return table;
        }

    }
}
