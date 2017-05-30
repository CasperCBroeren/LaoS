using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LaoS.Models
{
    public class AzureStorageRow<T> : TableEntity
    {
        public AzureStorageRow()
        {

        }

        public AzureStorageRow(string partitionKey, string rowKey)
        { 
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }
        public string BlobAddress { get; set; }

        public async Task<bool> SetItem(CloudBlobContainer container, T item)
        {
            var serializer = new JsonSerializer();
            CloudBlockBlob blob = container.GetBlockBlobReference(this.PartitionKey + this.RowKey);
            using (var stream = new MemoryStream())
            {
                var json = JsonConvert.SerializeObject(item);

                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    await blob.UploadFromStreamAsync(stream);
                    return true;
                }
            }
        }

        public async Task<T> GetItem(CloudBlobContainer container, List<T> result =null)
        {
            var serializer = new JsonSerializer();
            try
            {
                var blob = await container.GetBlobReferenceFromServerAsync(this.PartitionKey + this.RowKey);
                using (var stream = new MemoryStream())
                {
                    await blob.DownloadToStreamAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    var item = serializer.Deserialize<T>(new JsonTextReader(new StreamReader(stream)));
                    if (result != null)
                        result.Add(item);
                    return item;
                }
            }
            catch(Exception)
            {
                return default(T);
            }
        }

        public async Task RemoveItem(CloudBlobContainer container )
        {
            var serializer = new JsonSerializer();
            var blob = await container.GetBlobReferenceFromServerAsync(this.PartitionKey + this.RowKey);
            await blob.DeleteAsync(); 
        }
    }
}
