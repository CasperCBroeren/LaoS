using System.Collections.Generic;
using LaoS.Models;
using LaoS.Interfaces; 
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;

namespace LaoS.Services
{
    public class AzureStorageChannelMessageStore : IChannelMessageStore
    {
        private string _storageAccount;
        private string _storageKey;

        public AzureStorageChannelMessageStore(IAppSettings appSettings)
        { 
            this._storageAccount = appSettings.Get("storageAccount");
            this._storageKey = appSettings.Get("storageKey");
        }

        private async Task<CloudTable> GetTableRef()
        {
            StorageCredentials credentials = new StorageCredentials(_storageAccount, _storageKey);
            CloudStorageAccount account = new CloudStorageAccount(credentials, true);
            var client = account.CreateCloudTableClient();

            var tableRef = client.GetTableReference("laosMessages");
            await tableRef.CreateIfNotExistsAsync();

            return tableRef;
        }

        private async Task<CloudBlobContainer> GetContainerRef()
        {
            StorageCredentials credentials = new StorageCredentials(_storageAccount, _storageKey);
            CloudStorageAccount account = new CloudStorageAccount(credentials, true);
            var client = account.CreateCloudBlobClient();

            var containerRef = client.GetContainerReference("laosmessagesfull");
            await containerRef.CreateIfNotExistsAsync();

            return containerRef;
        }

        public async Task DeleteMessage(SlackMessage message)
        {
            var table = await GetTableRef();
            var container = await GetContainerRef();
        
            TableOperation retrieveOperation = TableOperation.Retrieve<AzureStorageRow<SlackMessage>>(message.Channel, message.Deleted_Ts.ToString(SlackMessage.DecimalFormat));
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);
            var deleteEntity = (AzureStorageRow<SlackMessage>)retrievedResult.Result;
            TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
            await deleteEntity.RemoveItem(container);
            await table.ExecuteAsync(deleteOperation);

        }

        public async Task<IReadOnlyList<SlackMessage>> GetAllPast(string channel, int amount)
        {
            var container = await GetContainerRef();
            var table = await GetTableRef(); 
            TableContinuationToken token = null;
            var entities = new List<AzureStorageRow<SlackMessage>>();
            do
            {
                var query = new TableQuery<AzureStorageRow<SlackMessage>>();
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null && entities.Count < amount);

            List<SlackMessage> result = new List<SlackMessage>();
            List<Task> items = new List<Task>();
            foreach(var item in entities)
            {
                items.Add(item.GetItem(container, result));
            }
            await Task.WhenAll(items);
            result.OrderByDescending(x => x.Ts);
            return result;
        }
        public async Task<bool> StoreMessage(SlackMessage message)
        {
            var container = await GetContainerRef();
            var table = await GetTableRef();

            var toStore = new AzureStorageRow<SlackMessage>(message.Channel, message.Event_Ts.ToString(SlackMessage.DecimalFormat));
            await toStore.SetItem(container, message);
            
            TableOperation insertOperation = TableOperation.InsertOrReplace(toStore);
            await table.ExecuteAsync(insertOperation);
            return true;
        }

        public async Task<SlackMessage> UpdateMessage(SlackMessage message)
        {
            var table = await GetTableRef();
            var container = await GetContainerRef();

            var tsOfPrev = message.Previous_Message != null ? message.Previous_Message.Ts : message.Message.Ts;
            var toUpdate = new AzureStorageRow<SlackMessage>(message.Channel, tsOfPrev.ToString(SlackMessage.DecimalFormat));
            var totalMessage = await toUpdate.GetItem(container);
           
            if (totalMessage != null)
            {
                var original = totalMessage;
                original.Text = message.Message.Text;
                original.Subtype = message.Subtype;
                original.Attachments = message.Message.Attachments;
                original.Previous_Message = message.Previous_Message;
                await this.StoreMessage(original);
                return original;
            }
            else
            {
               await this.StoreMessage(message);
            }
            return message;
        }
    }
}