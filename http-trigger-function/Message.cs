using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace HttpTriggerFunction1
{

    public class Message : TableEntity
    {
        public Message()
        {
            
        }

        public Message(string data)
        {
            this.PartitionKey = "test";
            this.RowKey = Guid.NewGuid().ToString();
            this.Data = data;
        }

        public string Data {get; set;}
    }
}