using Azure.Storage.Blobs;
using Azure.Identity;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class BlobWriter
{
    private readonly ILogger _logger;

    public BlobWriter(ILogger logger)
    {
        _logger = logger;
    }

    public async Task WriteToBlobAsync(string message)
    {
        _logger.LogInformation($"Begin {nameof(WriteToBlobAsync)}");

        var client = await GetClient();

        string fileName = $"message{DateTime.Now.ToString("yyyyMMddHHmmssFFF")}.txt";

        BlobClient blobClient = client.GetBlobClient(fileName);

        UnicodeEncoding uniEncoding = new UnicodeEncoding();
        byte[] s = uniEncoding.GetBytes(message);

        // Create and upload blob data to the storage account
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await memoryStream.WriteAsync(s, 0, s.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            await blobClient.UploadAsync(memoryStream);
            memoryStream.Close();
        }
    }

    public async Task<BlobContainerClient> GetClient()
    {        
        _logger.LogInformation("Getting Blob Client");

        // Create a BlobServiceClient 
        var endpointUri = Environment.GetEnvironmentVariable("BlobStorageEndpoint");
        BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(endpointUri), new DefaultAzureCredential());

        // Create the container client
        string containerName = "test";
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        
        if(await containerClient.ExistsAsync() == false)
        {
            // Create container if it doesn't exist
            _logger.LogInformation($"Create blob container: {containerName}");
            containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
        }

        return containerClient;
    }
}